using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(organization => organization.Id);
        builder.Property(organization => organization.Id).ValueGeneratedNever();

        builder.Property(organization => organization.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(organization => organization.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(organization => organization.Description)
            .HasMaxLength(1024);

        builder.Property(organization => organization.IsActive)
            .HasDefaultValue(true);

        builder.Property(organization => organization.CreatedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(organization => organization.UpdatedAtUtc)
            .HasColumnType("datetime2");

        builder.HasIndex(organization => organization.Code).IsUnique();
        builder.HasIndex(organization => organization.Name).IsUnique();
    }
}
