using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class TimesheetConfiguration : IEntityTypeConfiguration<Timesheet>
{
    public void Configure(EntityTypeBuilder<Timesheet> builder)
    {
        builder.ToTable("Timesheets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.WeekStartUtc).HasColumnType("date").IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.SubmittedAtUtc);
        builder.Property(x => x.ApprovedAtUtc);
        builder.Property(x => x.ManagerId);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => new { x.EmployeeId, x.WeekStartUtc }).IsUnique();

        builder.HasMany(x => x.Entries)
            .WithOne(e => e.Timesheet!)
            .HasForeignKey(e => e.TimesheetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

