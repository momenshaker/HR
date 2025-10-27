using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class TrainingCourseConfiguration : IEntityTypeConfiguration<TrainingCourse>
{
    public void Configure(EntityTypeBuilder<TrainingCourse> builder)
    {
        builder.ToTable("TrainingCourses");

        builder.HasKey(course => course.Id);
        builder.Property(course => course.Id).ValueGeneratedNever();

        builder.Property(course => course.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(course => course.Category)
            .HasMaxLength(100);

        builder.Property(course => course.Description)
            .HasMaxLength(2000);

        builder.Property(course => course.Instructor)
            .HasMaxLength(150);

        builder.Property(course => course.StartDate)
            .HasColumnType("date");

        builder.Property(course => course.EndDate)
            .HasColumnType("date");

        builder.Property(course => course.DeliveryMode)
            .HasMaxLength(100);
    }
}
