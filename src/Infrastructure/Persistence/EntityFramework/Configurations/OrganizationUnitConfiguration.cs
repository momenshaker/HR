using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class OrganizationUnitConfiguration : IEntityTypeConfiguration<OrganizationUnit>
{
    public void Configure(EntityTypeBuilder<OrganizationUnit> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("OrganizationUnits");

        builder.HasKey(unit => unit.Id);

        builder.Property(unit => unit.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(unit => unit.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(unit => unit.Type)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(unit => unit.Description)
            .HasMaxLength(500);

        builder.Property(unit => unit.Level)
            .HasDefaultValue(0);

        builder.Property(unit => unit.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(unit => unit.Code)
            .IsUnique();

        builder.HasIndex(unit => unit.ParentUnitId);
    }
}
