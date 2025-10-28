using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class SubscriptionEntitlementConfiguration : IEntityTypeConfiguration<SubscriptionEntitlement>
{
    public void Configure(EntityTypeBuilder<SubscriptionEntitlement> builder)
    {
        builder.ToTable("SubscriptionEntitlements");

        builder.HasKey(entitlement => entitlement.Id);
        builder.Property(entitlement => entitlement.Id).ValueGeneratedNever();

        builder.Property(entitlement => entitlement.PlanCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(entitlement => entitlement.FeatureKey)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entitlement => entitlement.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entitlement => entitlement.Description)
            .HasMaxLength(512);

        builder.Property(entitlement => entitlement.MeasurementUnit)
            .HasMaxLength(50);

        builder.Property(entitlement => entitlement.IsEnabled)
            .HasDefaultValue(true);

        builder.Property(entitlement => entitlement.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(entitlement => new { entitlement.PlanCode, entitlement.FeatureKey, entitlement.SubscriptionId });

        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(entitlement => entitlement.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
