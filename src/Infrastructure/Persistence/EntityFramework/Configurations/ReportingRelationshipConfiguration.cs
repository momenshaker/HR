using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class ReportingRelationshipConfiguration : IEntityTypeConfiguration<ReportingRelationship>
{
    public void Configure(EntityTypeBuilder<ReportingRelationship> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("ReportingRelationships");

        builder.HasKey(relationship => relationship.Id);

        builder.Property(relationship => relationship.RelationshipType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(relationship => relationship.IsPrimary)
            .HasDefaultValue(false);

        builder.HasIndex(relationship => relationship.ManagerPositionId);
        builder.HasIndex(relationship => relationship.ReportPositionId);
    }
}
