using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    public void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        builder.ToTable("PayrollRuns");

        builder.HasKey(payroll => payroll.Id);
        builder.Property(payroll => payroll.Id).ValueGeneratedNever();

        builder.Property(payroll => payroll.PeriodStart)
            .HasColumnType("date");

        builder.Property(payroll => payroll.PeriodEnd)
            .HasColumnType("date");

        builder.Property(payroll => payroll.ProcessedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(payroll => payroll.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(payroll => payroll.TotalGrossPay)
            .HasColumnType("decimal(18,2)");

        builder.Property(payroll => payroll.TotalNetPay)
            .HasColumnType("decimal(18,2)");

        builder.Property(payroll => payroll.Notes)
            .HasMaxLength(1024);
    }
}
