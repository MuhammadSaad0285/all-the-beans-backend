using AllTheBeans.Domain.Entities;
using AllTheBeans.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AllTheBeans.Infrastructure.Idempotency;

public class EfIdempotencyStore : Application.Abstractions.IIdempotencyStore
{
    private readonly AllTheBeansDbContext _context;

    public EfIdempotencyStore(AllTheBeansDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _context.IdempotencyKeys.AnyAsync(k => k.Key == key);
    }

    public async Task CreateKeyAsync(string key)
    {
        var entity = new IdempotencyKey(key);
        _context.IdempotencyKeys.Add(entity);
        await _context.SaveChangesAsync();
    }
}
