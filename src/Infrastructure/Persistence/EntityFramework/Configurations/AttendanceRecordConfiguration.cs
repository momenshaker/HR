using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");

        builder.HasKey(record => record.Id);
        builder.Property(record => record.Id).ValueGeneratedNever();

        builder.Property(record => record.EmployeeId)
            .IsRequired();

        builder.Property(record => record.WorkDate)
            .HasColumnType("date");

        builder.Property(record => record.ShiftName)
            .HasMaxLength(100);

        builder.Property(record => record.ScheduledStartTimeUtc)
            .HasColumnType("datetimeoffset");

        builder.Property(record => record.ScheduledEndTimeUtc)
            .HasColumnType("datetimeoffset");

        builder.Property(record => record.CheckInTimeUtc)
            .HasColumnType("datetimeoffset");

        builder.Property(record => record.CheckOutTimeUtc)
            .HasColumnType("datetimeoffset");

        builder.Property(record => record.ScheduledWorkMinutes)
            .HasDefaultValue(0);

        builder.Property(record => record.BreakMinutes)
            .HasDefaultValue(0);

        builder.Property(record => record.GracePeriodMinutes)
            .HasDefaultValue(0);

        builder.Property(record => record.TotalWorkedMinutes)
            .HasDefaultValue(0);

        builder.Property(record => record.LateMinutes)
            .HasDefaultValue(0);

        builder.Property(record => record.EarlyLeaveMinutes)
            .HasDefaultValue(0);

        builder.Property(record => record.OvertimeMinutes)
            .HasDefaultValue(0);

        builder.Property(record => record.AbsenceMinutes)
            .HasDefaultValue(0);

        builder.HasMany(record => record.Punches)
            .WithOne(punch => punch.AttendanceRecord)
            .HasForeignKey(punch => punch.AttendanceRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(record => record.Punches).AutoInclude();

        builder.Property(record => record.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(record => record.Source)
            .HasMaxLength(50)
            .HasDefaultValue("Manual");

        builder.Property(record => record.Remarks)
            .HasMaxLength(1024);

        builder.HasIndex(record => new { record.EmployeeId, record.WorkDate }).IsUnique();
    }
}
