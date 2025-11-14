using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class EmployeeScheduleConfiguration : IEntityTypeConfiguration<EmployeeSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeSchedule> builder)
    {
        builder.ToTable("EmployeeSchedules");
        builder.HasKey(schedule => schedule.Id);
        builder.Property(schedule => schedule.Id).ValueGeneratedNever();

        builder.Property(schedule => schedule.EmployeeId)
            .IsRequired();

        builder.Property(schedule => schedule.WorkScheduleId)
            .IsRequired();

        builder.Property(schedule => schedule.EffectiveFrom)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(schedule => schedule.EffectiveTo)
            .HasColumnType("date");

        builder.HasIndex(schedule => new { schedule.EmployeeId, schedule.EffectiveFrom });

        builder.HasOne(schedule => schedule.WorkSchedule)
            .WithMany()
            .HasForeignKey(schedule => schedule.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
