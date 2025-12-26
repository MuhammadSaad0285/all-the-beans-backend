using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllTheBeans.Infrastructure.Persistence.Configurations;

public class IdempotencyKeyConfig : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        builder.ToTable("IdempotencyKeys");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Key).IsRequired().HasMaxLength(50);
        builder.Property(i => i.CreatedAt).IsRequired();
        builder.HasIndex(i => i.Key).IsUnique();
    }
}
