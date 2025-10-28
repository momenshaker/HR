using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class CourseEnrollmentConfiguration : IEntityTypeConfiguration<CourseEnrollment>
{
    public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
    {
        builder.ToTable("CourseEnrollments");

        builder.HasKey(enrollment => enrollment.Id);
        builder.Property(enrollment => enrollment.Id).ValueGeneratedNever();

        builder.Property(enrollment => enrollment.EnrolledOn).HasColumnType("date");
        builder.Property(enrollment => enrollment.CompletedOn).HasColumnType("date");

        builder.Property(enrollment => enrollment.Status)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(enrollment => enrollment.CompletionPercentage)
            .HasColumnType("decimal(5,2)");

        builder.HasIndex(enrollment => new { enrollment.CourseId, enrollment.EmployeeId }).IsUnique();
    }
}
