using AllTheBeans.Application.Abstractions;
using AllTheBeans.Application.Errors;
using AllTheBeans.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using AllTheBeans.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Application.Services;

public class BeanOfTheDayService : IBeanOfTheDayService
{
    private readonly AllTheBeansDbContext _context;
    private readonly IClock _clock;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "BeanOfTheDay";

    public BeanOfTheDayService(AllTheBeansDbContext context, IClock clock, IMemoryCache cache)
    {
        _context = context;
        _clock = clock;
        _cache = cache;
    }

    public async Task<BeanOfTheDay> GetBeanOfTheDayAsync()
    {
        DateTime today = _clock.UtcNow.Date;
        // First, check if we have a BeanOfTheDay for today in cache
        if (_cache.TryGetValue(CacheKey, out BeanOfTheDay cachedBotd) && cachedBotd.Date == today)
        {
            return cachedBotd;
        }

        // Not in cache or stale, check database for today's record
        var existing = await _context.BeanOfTheDays
                                     .Include(b => b.Bean)
                                     .FirstOrDefaultAsync(b => b.Date == today);
        if (existing != null)
        {
            // Store in cache and return
            _cache.Set(CacheKey, existing, GetCacheExpiryForMidnight());
            return existing;
        }

        // No record for today, select a new Bean of the Day
        var allBeans = await _context.Beans.ToListAsync();
        if (allBeans.Count == 0)
        {
            throw new NotFoundException("No beans available for Bean of the Day.");
        }
        // Determine the last selected bean (yesterday)
        var lastRecord = await _context.BeanOfTheDays.OrderByDescending(b => b.Date).FirstOrDefaultAsync();
        int? lastBeanId = lastRecord?.BeanId;

        // Pick a random bean that is not the same as yesterday's (if possible)
        var rand = new Random();
        Bean chosenBean;
        if (allBeans.Count == 1)
        {
            // Only one bean available
            chosenBean = allBeans[0];
        }
        else
        {
            int index = rand.Next(allBeans.Count);
            if (lastBeanId.HasValue && allBeans[index].Id == lastBeanId.Value)
            {
                // avoid picking the same as last time
                index = (index + 1) % allBeans.Count;
            }
            chosenBean = allBeans[index];
        }

        // Create new BeanOfTheDay record for today
        var newRecord = new BeanOfTheDay
        {
            Date = today,
            BeanId = chosenBean.Id,
            Bean = chosenBean  // attach bean object
        };
        _context.BeanOfTheDays.Add(newRecord);
        await _context.SaveChangesAsync();

        // Cache the new record until end of day
        _cache.Set(CacheKey, newRecord, GetCacheExpiryForMidnight());
        return newRecord;
    }

    private MemoryCacheEntryOptions GetCacheExpiryForMidnight()
    {
        // Calculate time remaining until next midnight (UTC)
        DateTime tomorrow = _clock.UtcNow.Date.AddDays(1);
        TimeSpan expireTime = tomorrow - _clock.UtcNow;
        return new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expireTime };
    }
}
