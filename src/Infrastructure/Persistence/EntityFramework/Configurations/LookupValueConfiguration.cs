using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class LookupValueConfiguration : IEntityTypeConfiguration<LookupValue>
{
    public void Configure(EntityTypeBuilder<LookupValue> builder)
    {
        builder.ToTable("LookupValues");
        builder.HasKey(value => value.Id);

        builder.Property(value => value.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(value => value.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(value => value.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(value => value.Description)
            .HasMaxLength(512);

        builder.Property(value => value.SortOrder)
            .IsRequired();

        builder.Property(value => value.IsActive)
            .IsRequired();

        builder.Property(value => value.CreatedAtUtc)
            .IsRequired();

        builder.Property(value => value.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(value => new { value.Category, value.Code })
            .IsUnique();
    }
}
