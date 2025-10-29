using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(subscription => subscription.Id);
        builder.Property(subscription => subscription.Id).ValueGeneratedNever();

        builder.Property(subscription => subscription.PlanCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(subscription => subscription.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(SubscriptionStatus.Active);

        builder.Property(subscription => subscription.BillingInterval)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(subscription => subscription.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(subscription => subscription.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(subscription => subscription.AutoRenew)
            .HasDefaultValue(true);

        builder.Property(subscription => subscription.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(subscription => subscription.UpdatedAtUtc);

        builder.Property(subscription => subscription.StartDate)
            .IsRequired();

        builder.HasIndex(subscription => new { subscription.CustomerId, subscription.PlanCode });

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(subscription => subscription.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
