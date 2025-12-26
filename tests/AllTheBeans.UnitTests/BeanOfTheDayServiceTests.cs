using AllTheBeans.Domain.Entities;
using AllTheBeans.Infrastructure.Persistence;
using AllTheBeans.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllTheBeans.UnitTests;

public class BeanOfTheDayServiceTests
{
    private AllTheBeansDbContext CreateContext() =>
        new AllTheBeansDbContext(new DbContextOptionsBuilder<AllTheBeansDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetBeanOfTheDay_FirstCall_SelectsAndStoresBean()
    {
        var context = CreateContext();
        context.Beans.Add(new Bean { Name = "Bean1", Colour = "dark", Country = "X", Description = "", ImageUrl = "", Cost = new Domain.ValueObjects.Money(10, "GBP") });
        context.Beans.Add(new Bean { Name = "Bean2", Colour = "dark", Country = "X", Description = "", ImageUrl = "", Cost = new Domain.ValueObjects.Money(20, "GBP") });
        context.SaveChanges();
        var memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
        var service = new Application.Services.BeanOfTheDayService(context, new SystemClock(), (Microsoft.Extensions.Caching.Memory.IMemoryCache)memoryCache);

        var result = await service.GetBeanOfTheDayAsync();
        Assert.NotNull(result);
        Assert.NotNull(result.Bean);
        int countToday = context.BeanOfTheDays.Count(b => b.Date == DateTime.UtcNow.Date);
        Assert.Equal(1, countToday);
    }

    [Fact]
    public async Task GetBeanOfTheDay_TwoConsecutiveDays_PicksDifferentBean()
    {
        var context = CreateContext();
        var beanA = new Bean { Name = "A", Colour = "", Country = "", Description = "", ImageUrl = "", Cost = new Domain.ValueObjects.Money(5, "GBP") };
        var beanB = new Bean { Name = "B", Colour = "", Country = "", Description = "", ImageUrl = "", Cost = new Domain.ValueObjects.Money(6, "GBP") };
        context.Beans.AddRange(beanA, beanB);
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        context.BeanOfTheDays.Add(new BeanOfTheDay { Date = yesterday, BeanId = 0, Bean = beanA });
        context.SaveChanges();

        var clock = new TestClock { UtcNowValue = DateTime.UtcNow }; // custom clock for control
        var service = new Application.Services.BeanOfTheDayService(context, clock, (Microsoft.Extensions.Caching.Memory.IMemoryCache)new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions()));

        var botdToday = await service.GetBeanOfTheDayAsync();
        var firstBeanId = botdToday.BeanId;

        // Simulate next day
        clock.UtcNowValue = clock.UtcNowValue.AddDays(1);
        var botdTomorrow = await service.GetBeanOfTheDayAsync();
        var secondBeanId = botdTomorrow.BeanId;

        Assert.NotEqual(firstBeanId, secondBeanId);
    }

    private class TestClock : AllTheBeans.Application.Abstractions.IClock
    {
        public DateTime UtcNowValue { get; set; }
        public DateTime UtcNow => UtcNowValue;
    }
}
