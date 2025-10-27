using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class AnalyticsSnapshotConfiguration : IEntityTypeConfiguration<AnalyticsSnapshot>
{
    public void Configure(EntityTypeBuilder<AnalyticsSnapshot> builder)
    {
        builder.ToTable("AnalyticsSnapshots");

        builder.HasKey(snapshot => snapshot.Id);
        builder.Property(snapshot => snapshot.Id).ValueGeneratedNever();

        builder.Property(snapshot => snapshot.CapturedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(snapshot => snapshot.TurnoverRate)
            .HasColumnType("decimal(6,2)");

        builder.Property(snapshot => snapshot.AverageTenureMonths)
            .HasColumnType("decimal(6,2)");

        builder.Property(snapshot => snapshot.HiringVelocity)
            .HasColumnType("decimal(6,2)");

        builder.Property(snapshot => snapshot.EngagementScore)
            .HasColumnType("decimal(6,2)");

        builder.Property(snapshot => snapshot.Commentary)
            .HasMaxLength(2000);
    }
}
