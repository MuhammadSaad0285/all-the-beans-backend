using AllTheBeans.Domain.Entities;
using AllTheBeans.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllTheBeans.Infrastructure.Persistence.Configurations;

public class BeanConfig : IEntityTypeConfiguration<Bean>
{
    public void Configure(EntityTypeBuilder<Bean> builder)
    {
        builder.ToTable("Beans");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Colour).HasMaxLength(50);
        builder.Property(b => b.Country).HasMaxLength(50);
        builder.Property(b => b.Description).HasColumnType("TEXT");
        builder.Property(b => b.ImageUrl).HasMaxLength(200);
        // Configure owned type Money for Cost
        builder.OwnsOne(b => b.Cost, money =>
        {
            money.Property(m => m.Amount).HasColumnName("CostAmount").HasColumnType("DECIMAL(10,2)");
            money.Property(m => m.Currency).HasColumnName("CostCurrency").HasMaxLength(3);
        });
    }
}
