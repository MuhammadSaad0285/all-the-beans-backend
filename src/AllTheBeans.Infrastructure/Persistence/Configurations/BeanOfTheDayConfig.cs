using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllTheBeans.Infrastructure.Persistence.Configurations;

public class BeanOfTheDayConfig : IEntityTypeConfiguration<BeanOfTheDay>
{
    public void Configure(EntityTypeBuilder<BeanOfTheDay> builder)
    {
        builder.ToTable("BeanOfTheDay");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Date).IsRequired();
        builder.HasIndex(b => b.Date).IsUnique();
        builder.HasOne(b => b.Bean)
               .WithMany() // no navigation in Bean for BeanOfTheDay
               .HasForeignKey(b => b.BeanId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
