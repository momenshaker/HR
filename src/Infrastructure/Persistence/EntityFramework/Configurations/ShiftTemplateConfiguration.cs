using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class ShiftTemplateConfiguration : IEntityTypeConfiguration<ShiftTemplate>
{
    public void Configure(EntityTypeBuilder<ShiftTemplate> builder)
    {
        builder.ToTable("ShiftTemplates");
        builder.HasKey(template => template.Id);
        builder.Property(template => template.Id).ValueGeneratedNever();

        builder.Property(template => template.DayOfWeek)
            .IsRequired();

        builder.Property(template => template.StartTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(template => template.EndTime)
            .HasColumnType("time")
            .IsRequired();

        builder.Property(template => template.BreakMinutes)
            .HasDefaultValue(0);

        builder.Property(template => template.GracePeriodMinutes)
            .HasDefaultValue(0);

        builder.Property(template => template.MinimumOvertimeMinutes)
            .HasDefaultValue(0);
    }
}
