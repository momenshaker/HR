using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.Id).ValueGeneratedNever();

        builder.Property(customer => customer.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(customer => customer.BillingEmail)
            .HasMaxLength(320);

        builder.Property(customer => customer.BillingPhone)
            .HasMaxLength(30);

        builder.Property(customer => customer.AddressLine1)
            .HasMaxLength(200);

        builder.Property(customer => customer.AddressLine2)
            .HasMaxLength(200);

        builder.Property(customer => customer.City)
            .HasMaxLength(100);

        builder.Property(customer => customer.State)
            .HasMaxLength(100);

        builder.Property(customer => customer.PostalCode)
            .HasMaxLength(20);

        builder.Property(customer => customer.Country)
            .HasMaxLength(100);

        builder.Property(customer => customer.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Active");

        builder.Property(customer => customer.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(customer => customer.UpdatedAtUtc);

        builder.Property(customer => customer.TrialEndsOn);

        builder.HasIndex(customer => customer.BillingEmail);
    }
}
