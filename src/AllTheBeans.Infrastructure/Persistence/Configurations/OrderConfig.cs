using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllTheBeans.Infrastructure.Persistence.Configurations;

public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CreatedAt).IsRequired();
        // One-to-many: Order -> OrderLine
        builder.HasMany(o => o.Lines)
               .WithOne(l => l.Order!)
               .HasForeignKey(l => l.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
