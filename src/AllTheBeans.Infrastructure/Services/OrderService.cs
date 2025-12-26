using AllTheBeans.Application.Abstractions;
using AllTheBeans.Application.Errors;
using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AllTheBeans.Infrastructure.Persistence;
namespace AllTheBeans.Application.Services;

public class OrderService : IOrderService
{
    private readonly AllTheBeansDbContext _context;
    private readonly IBeanService _beanService;
    private readonly IIdempotencyStore _idempotencyStore;
    private readonly IClock _clock;

    public OrderService(AllTheBeansDbContext context, IBeanService beanService, IIdempotencyStore idempotencyStore, IClock clock)
    {
        _context = context;
        _beanService = beanService;
        _idempotencyStore = idempotencyStore;
        _clock = clock;
    }

    public async Task<Order> CreateOrderAsync(IEnumerable<(int beanId, int quantity)> items, string? idempotencyKey)
    {
        // Idempotency check
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            if (await _idempotencyStore.ExistsAsync(idempotencyKey))
            {
                throw new DuplicateRequestException("This request was already processed.");
            }
            // Reserve idempotency key (insert into store)
            await _idempotencyStore.CreateKeyAsync(idempotencyKey);
        }

        var order = new Order { CreatedAt = _clock.UtcNow };
        foreach (var (beanId, quantity) in items)
        {
            var bean = await _beanService.GetBeanAsync(beanId);
            if (quantity <= 0)
            {
                throw new ValidationException($"Quantity for Bean {beanId} must be at least 1.");
            }
            var line = new OrderLine
            {
                BeanId = bean.Id,
                Bean = bean,
                Quantity = quantity,
                UnitPrice = new Domain.ValueObjects.Money(bean.Cost.Amount, bean.Cost.Currency)
            };
            order.Lines.Add(line);
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        var order = await _context.Orders
                                  .Include(o => o.Lines)
                                    .ThenInclude(l => l.Bean)
                                  .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new NotFoundException($"Order with id {id} was not found.");
        return order;
    }
}
