using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(log => log.Id);
        builder.Property(log => log.Id).ValueGeneratedNever();

        builder.Property(log => log.Actor)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(log => log.ActorEmail)
            .HasMaxLength(320);

        builder.Property(log => log.EntityName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(log => log.EntityId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(log => log.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(log => log.Severity)
            .HasMaxLength(50)
            .HasDefaultValue("Info");

        builder.Property(log => log.CorrelationId)
            .HasMaxLength(100);

        builder.Property(log => log.Source)
            .HasMaxLength(100);

        builder.Property(log => log.Changes)
            .HasMaxLength(4000);

        builder.Property(log => log.Metadata)
            .HasMaxLength(2000);

        builder.Property(log => log.OccurredAtUtc)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(log => log.OccurredAtUtc);
        builder.HasIndex(log => log.CustomerId);
    }
}
