using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("Announcements");

        builder.HasKey(announcement => announcement.Id);
        builder.Property(announcement => announcement.Id).ValueGeneratedNever();

        builder.Property(announcement => announcement.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(announcement => announcement.Message)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(announcement => announcement.Audience)
            .HasMaxLength(200);

        builder.Property(announcement => announcement.CreatedBy)
            .IsRequired();

        builder.Property(announcement => announcement.PublishedAtUtc)
            .HasColumnType("datetime2");
    }
}
