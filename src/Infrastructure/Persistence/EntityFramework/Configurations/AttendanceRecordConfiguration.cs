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

        builder.HasMany(record => record.Punches)
            .WithOne(punch => punch.AttendanceRecord)
            .HasForeignKey(punch => punch.AttendanceRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(record => record.Punches).AutoInclude();

        builder.Property(record => record.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(record => record.Notes)
            .HasMaxLength(1024);

        builder.HasIndex(record => new { record.EmployeeId, record.WorkDate }).IsUnique();
    }
}
