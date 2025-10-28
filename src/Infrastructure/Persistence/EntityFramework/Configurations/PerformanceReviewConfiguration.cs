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

        builder.OwnsMany(review => review.Goals, goals =>
        {
            goals.ToTable("PerformanceGoals");
            goals.WithOwner().HasForeignKey("PerformanceReviewId");

            goals.HasKey(goal => goal.Id);
            goals.Property(goal => goal.Id).ValueGeneratedNever();

            goals.Property(goal => goal.Title)
                .IsRequired()
                .HasMaxLength(200);

            goals.Property(goal => goal.Description)
                .HasMaxLength(2000);

            goals.Property(goal => goal.Weight)
                .HasColumnType("decimal(5,2)");

            goals.Property(goal => goal.Alignment)
                .HasMaxLength(100);

            goals.Property(goal => goal.Status)
                .HasMaxLength(50);
        });

        builder.OwnsMany(review => review.KeyPerformanceIndicators, kpis =>
        {
            kpis.ToTable("PerformanceKpis");
            kpis.WithOwner().HasForeignKey("PerformanceReviewId");

            kpis.HasKey(kpi => kpi.Id);
            kpis.Property(kpi => kpi.Id).ValueGeneratedNever();

            kpis.Property(kpi => kpi.Name)
                .IsRequired()
                .HasMaxLength(150);

            kpis.Property(kpi => kpi.TargetValue)
                .HasColumnType("decimal(18,2)");

            kpis.Property(kpi => kpi.ActualValue)
                .HasColumnType("decimal(18,2)");

            kpis.Property(kpi => kpi.UnitOfMeasure)
                .HasMaxLength(50);

            kpis.Property(kpi => kpi.Status)
                .HasMaxLength(50);
        });

        builder.OwnsMany(review => review.FeedbackCycles, feedbacks =>
        {
            feedbacks.ToTable("PerformanceFeedback");
            feedbacks.WithOwner().HasForeignKey("PerformanceReviewId");

            feedbacks.HasKey(feedback => feedback.Id);
            feedbacks.Property(feedback => feedback.Id).ValueGeneratedNever();

            feedbacks.Property(feedback => feedback.FeedbackType)
                .IsRequired()
                .HasMaxLength(50);

            feedbacks.Property(feedback => feedback.Comments)
                .IsRequired()
                .HasMaxLength(2000);

            feedbacks.Property(feedback => feedback.SubmittedAtUtc)
                .HasColumnType("datetime2");
        });

        builder.OwnsOne(review => review.CompensationReview, compensation =>
        {
            compensation.ToTable("PerformanceCompensationReviews");

            compensation.Property(c => c.EffectiveDate)
                .HasColumnType("date");

            compensation.Property(c => c.CurrentBaseSalary)
                .HasColumnType("decimal(18,2)");

            compensation.Property(c => c.ProposedBaseSalary)
                .HasColumnType("decimal(18,2)");

            compensation.Property(c => c.BonusRecommendation)
                .HasColumnType("decimal(18,2)");

            compensation.Property(c => c.Currency)
                .HasMaxLength(3);

            compensation.Property(c => c.Notes)
                .HasMaxLength(2000);
        });
    }
}
