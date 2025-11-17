using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class AttendancePunchEntityConfiguration : IEntityTypeConfiguration<AttendancePunch>
{
    public void Configure(EntityTypeBuilder<AttendancePunch> builder)
    {
        builder.ToTable("AttendancePunches");

        builder.HasKey(punch => punch.Id);
        builder.Property(punch => punch.Id).ValueGeneratedNever();

        builder.Property(punch => punch.AttendanceRecordId).IsRequired();

        builder.Property(punch => punch.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(punch => punch.TimestampUtc)
            .IsRequired()
            .HasColumnType("datetimeoffset");

        builder.Property(punch => punch.Source)
            .HasMaxLength(100);

        builder.Property(punch => punch.DeviceId)
            .HasMaxLength(100);

        builder.Property(punch => punch.Location)
            .HasMaxLength(200);

        builder.Property(punch => punch.Notes)
            .HasMaxLength(500);

        builder.HasOne(punch => punch.AttendanceRecord)
            .WithMany(record => record.Punches)
            .HasForeignKey(punch => punch.AttendanceRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
