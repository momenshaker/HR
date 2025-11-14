using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.ToTable("Holidays");
        builder.HasKey(holiday => holiday.Id);
        builder.Property(holiday => holiday.Id).ValueGeneratedNever();

        builder.Property(holiday => holiday.OrganizationId)
            .IsRequired();

        builder.Property(holiday => holiday.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(holiday => holiday.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(holiday => holiday.CountryCode)
            .HasMaxLength(10);

        builder.Property(holiday => holiday.Description)
            .HasMaxLength(500);

        builder.HasIndex(holiday => new { holiday.OrganizationId, holiday.Date })
            .IsUnique();
    }
}
