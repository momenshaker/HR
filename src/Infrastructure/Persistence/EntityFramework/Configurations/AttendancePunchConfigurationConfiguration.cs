using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class AttendancePunchConfigurationConfiguration : IEntityTypeConfiguration<AttendancePunchConfiguration>
{
    public void Configure(EntityTypeBuilder<AttendancePunchConfiguration> builder)
    {
        builder.ToTable("AttendancePunchConfigurations");

        builder.HasKey(configuration => configuration.Id);
        builder.Property(configuration => configuration.Id).ValueGeneratedNever();

        builder.Property(configuration => configuration.PunchType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(configuration => configuration.DisplayName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(configuration => configuration.Description)
            .HasMaxLength(500);

        builder.Property(configuration => configuration.SortOrder)
            .HasDefaultValue(0);

        builder.Property(configuration => configuration.IsActive)
            .HasDefaultValue(true);
    }
}
