using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class PerformanceReviewConfiguration : IEntityTypeConfiguration<PerformanceReview>
{
    public void Configure(EntityTypeBuilder<PerformanceReview> builder)
    {
        builder.ToTable("PerformanceReviews");

        builder.HasKey(review => review.Id);
        builder.Property(review => review.Id).ValueGeneratedNever();

        builder.Property(review => review.EmployeeId)
            .IsRequired();

        builder.Property(review => review.CycleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(review => review.PeriodStart)
            .HasColumnType("date");

        builder.Property(review => review.PeriodEnd)
            .HasColumnType("date");

        builder.Property(review => review.OverallScore)
            .HasColumnType("decimal(5,2)");

        builder.Property(review => review.ManagerComments)
            .HasMaxLength(2000);

        builder.Property(review => review.GoalsSummary)
            .HasMaxLength(2000);

        builder.Property(review => review.SubmittedAtUtc)
            .HasColumnType("datetime2");

        builder.HasIndex(review => new { review.EmployeeId, review.CycleName }).IsUnique();
    }
}
