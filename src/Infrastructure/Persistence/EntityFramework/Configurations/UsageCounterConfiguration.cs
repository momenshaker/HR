using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class UsageCounterConfiguration : IEntityTypeConfiguration<UsageCounter>
{
    public void Configure(EntityTypeBuilder<UsageCounter> builder)
    {
        builder.ToTable("UsageCounters");

        builder.HasKey(counter => counter.Id);
        builder.Property(counter => counter.Id).ValueGeneratedNever();

        builder.Property(counter => counter.MetricKey)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(counter => counter.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(counter => counter.MeasurementUnit)
            .HasMaxLength(50);

        builder.Property(counter => counter.CurrentValue)
            .HasColumnType("decimal(18,4)");

        builder.Property(counter => counter.Limit)
            .HasColumnType("decimal(18,4)");

        builder.Property(counter => counter.LastResetAtUtc)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(counter => counter.UpdatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(counter => counter.PeriodStart).IsRequired();
        builder.Property(counter => counter.PeriodEnd).IsRequired();

        builder.HasIndex(counter => new { counter.SubscriptionId, counter.MetricKey, counter.PeriodStart, counter.PeriodEnd })
            .IsUnique();

        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(counter => counter.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
