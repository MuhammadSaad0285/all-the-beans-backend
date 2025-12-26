using AllTheBeans.Application.Errors;
using AllTheBeans.Application.Services;
using AllTheBeans.Domain.Entities;
using AllTheBeans.Infrastructure.Idempotency;
using AllTheBeans.Infrastructure.Persistence;
using AllTheBeans.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllTheBeans.UnitTests;

public class OrderServiceTests
{
    private AllTheBeansDbContext CreateContext() =>
        new AllTheBeansDbContext(new DbContextOptionsBuilder<AllTheBeansDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task CreateOrder_ComputesTotalCorrectly()
    {
        var context = CreateContext();
        var bean1 = new Bean { Name = "Bean1", Cost = new AllTheBeans.Domain.ValueObjects.Money(5.0m, "GBP") };
        var bean2 = new Bean { Name = "Bean2", Cost = new AllTheBeans.Domain.ValueObjects.Money(2.0m, "GBP") };
        context.Beans.AddRange(bean1, bean2);
        context.SaveChanges();
        var beanService = new BeanService(context);
        var orderService = new OrderService(context, beanService, new DummyIdempotencyStore(), new SystemClock());

        var items = new List<(int, int)> { (bean1.Id, 2), (bean2.Id, 3) };
        var order = await orderService.CreateOrderAsync(items, null);

        Assert.NotNull(order);
        Assert.Equal(2, order.Lines.Count);
        //Calculate expected total: 2*5 + 3*2 = 10 + 6 = 16
        decimal expectedTotal = 16.0m;
        decimal actualTotal = order.Lines.Sum(l => l.UnitPrice.Amount * l.Quantity);
        Assert.Equal(expectedTotal, actualTotal);
        var line1 = order.Lines.First(l => l.BeanId == bean1.Id);
        Assert.Equal(bean1.Cost.Amount, line1.UnitPrice.Amount);
        var line2 = order.Lines.First(l => l.BeanId == bean2.Id);
        Assert.Equal(bean2.Cost.Amount, line2.UnitPrice.Amount);
    }

    [Fact]
    public async Task CreateOrder_DuplicateIdempotencyKey_ThrowsDuplicateRequestException()
    {
        var context = CreateContext();

        // Seed a bean and keep a reference so we can use its generated Id
        var bean = new Bean
        {
            Name = "TestBean",
            Cost = new AllTheBeans.Domain.ValueObjects.Money(1.0m, "GBP")
        };

        context.Beans.Add(bean);
        context.SaveChanges();

        var beanService = new BeanService(context);
        var idStore = new EfIdempotencyStore(context);
        var orderService = new OrderService(context, beanService, idStore, new SystemClock());

        // Use the seeded bean Id (NOT 0)
        var items = new List<(int, int)> { (bean.Id, 1) };
        string key = "ABC123";

        // First call - should succeed
        var order = await orderService.CreateOrderAsync(items, key);
        Assert.NotNull(order);

        // Second call with same key - should throw DuplicateRequestException
        await Assert.ThrowsAsync<DuplicateRequestException>(() => orderService.CreateOrderAsync(items, key));

        // Ensure only one order in database
        Assert.Equal(1, context.Orders.Count());
    }

    // Dummy idempotency store for tests (always indicates no prior keys, does nothing on create)
    private class DummyIdempotencyStore : AllTheBeans.Application.Abstractions.IIdempotencyStore
    {
        public Task<bool> ExistsAsync(string key) => Task.FromResult(false);
        public Task CreateKeyAsync(string key) => Task.CompletedTask;
    }
}
