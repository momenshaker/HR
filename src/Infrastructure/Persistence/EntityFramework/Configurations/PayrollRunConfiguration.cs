using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    public void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        builder.ToTable("PayrollRuns");

        builder.HasKey(run => run.Id);
        builder.Property(run => run.Id).ValueGeneratedNever();

        builder.Property(run => run.OrganizationId)
            .IsRequired();

        builder.Property(run => run.PeriodStart)
            .HasColumnType("date");

        builder.Property(run => run.PeriodEnd)
            .HasColumnType("date");

        builder.Property(run => run.PayDate)
            .HasColumnType("date");

        builder.Property(run => run.CreatedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(run => run.ApprovedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(run => run.PaidAtUtc)
            .HasColumnType("datetime2");

        builder.Property(run => run.Status)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(run => run.TotalGrossPay)
            .HasColumnType("decimal(18,2)");

        builder.Property(run => run.TotalNetPay)
            .HasColumnType("decimal(18,2)");

        builder.Property(run => run.Notes)
            .HasMaxLength(1024);

        builder.Property(run => run.RowVersion)
            .IsRowVersion();

        builder.HasMany(run => run.Items)
            .WithOne(item => item.Run!)
            .HasForeignKey(item => item.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(run => run.Payslips)
            .WithOne(ps => ps.Run!)
            .HasForeignKey(ps => ps.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        // Common analytics filters: org and period
        builder.HasIndex(run => new { run.OrganizationId, run.PeriodStart, run.PeriodEnd });
    }
}
