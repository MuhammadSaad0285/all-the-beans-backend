using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Application.Abstractions;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(IEnumerable<(int beanId, int quantity)> items, string? idempotencyKey);
    Task<Order> GetOrderAsync(int id);
}
