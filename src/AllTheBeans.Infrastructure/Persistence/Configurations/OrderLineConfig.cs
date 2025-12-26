using AllTheBeans.Domain.Entities;
using AllTheBeans.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllTheBeans.Infrastructure.Persistence.Configurations;

public class OrderLineConfig : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("OrderLines");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Quantity).IsRequired();
        // Configure owned Money for UnitPrice
        builder.OwnsOne(l => l.UnitPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("UnitPriceAmount").HasColumnType("DECIMAL(10,2)");
            money.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3);
        });
        builder.HasOne(l => l.Bean)
               .WithMany()  // no navigation in Bean for OrderLines
               .HasForeignKey(l => l.BeanId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
