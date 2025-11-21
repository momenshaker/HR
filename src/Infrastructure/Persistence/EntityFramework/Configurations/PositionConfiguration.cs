using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Positions");

        builder.HasKey(position => position.Id);

        builder.Property(position => position.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(position => position.JobCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(position => position.Grade)
            .HasMaxLength(20);

        builder.Property(position => position.EmploymentType)
            .HasMaxLength(40);

        builder.Property(position => position.IsVacant)
            .HasDefaultValue(false);

        builder.Property(position => position.IsCriticalRole)
            .HasDefaultValue(false);

        builder.HasIndex(position => position.OrganizationUnitId);
        builder.HasIndex(position => position.ReportsToPositionId);
    }
}
