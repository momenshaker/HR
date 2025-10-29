using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(invoice => invoice.Id);
        builder.Property(invoice => invoice.Id).ValueGeneratedNever();

        builder.Property(invoice => invoice.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(invoice => invoice.InvoiceNumber).IsUnique();

        builder.Property(invoice => invoice.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(invoice => invoice.Status)
            .IsRequired()
            .HasDefaultValue(InvoiceStatus.Draft);

        builder.Property(invoice => invoice.Notes)
            .HasMaxLength(1024);

        builder.Property(invoice => invoice.Subtotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(invoice => invoice.TaxTotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(invoice => invoice.Total)
            .HasColumnType("decimal(18,2)");

        builder.Property(invoice => invoice.AmountPaid)
            .HasColumnType("decimal(18,2)");

        builder.Property(invoice => invoice.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(invoice => invoice.UpdatedAtUtc);

        builder.Property(invoice => invoice.IssueDate).IsRequired();
        builder.Property(invoice => invoice.DueDate).IsRequired();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(invoice => invoice.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(invoice => invoice.SubscriptionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
