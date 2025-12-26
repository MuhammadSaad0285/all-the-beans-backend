using AllTheBeans.Domain.Entities;
using AllTheBeans.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Infrastructure.Persistence;

public class AllTheBeansDbContext : DbContext
{
    public AllTheBeansDbContext(DbContextOptions<AllTheBeansDbContext> options) : base(options) { }

    public DbSet<Bean> Beans => Set<Bean>();
    public DbSet<BeanOfTheDay> BeanOfTheDays => Set<BeanOfTheDay>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configuration classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AllTheBeansDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
