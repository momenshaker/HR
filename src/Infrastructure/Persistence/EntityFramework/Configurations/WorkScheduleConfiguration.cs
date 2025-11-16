using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.ToTable("WorkSchedules");
        builder.HasKey(schedule => schedule.Id);
        builder.Property(schedule => schedule.Id).ValueGeneratedNever();

        builder.Property(schedule => schedule.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(schedule => schedule.TimeZoneId)
            .HasMaxLength(100);

        builder.HasMany(schedule => schedule.ShiftTemplates)
            .WithOne(template => template.WorkSchedule)
            .HasForeignKey(template => template.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
