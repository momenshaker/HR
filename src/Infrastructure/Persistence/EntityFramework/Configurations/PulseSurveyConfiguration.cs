using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class PulseSurveyConfiguration : IEntityTypeConfiguration<PulseSurvey>
{
    public void Configure(EntityTypeBuilder<PulseSurvey> builder)
    {
        builder.ToTable("PulseSurveys");

        builder.HasKey(survey => survey.Id);
        builder.Property(survey => survey.Id).ValueGeneratedNever();

        builder.Property(survey => survey.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(survey => survey.Description)
            .HasMaxLength(2000);

        builder.Property(survey => survey.Audience)
            .HasMaxLength(200);

        builder.Property(survey => survey.QuestionSet)
            .HasMaxLength(4000);

        builder.Property(survey => survey.ResponseWindowMinutes)
            .IsRequired();

        builder.Property(survey => survey.OwnerId)
            .IsRequired();

        builder.Property(survey => survey.LaunchDateUtc)
            .HasColumnType("datetime2");

        builder.Property(survey => survey.CloseDateUtc)
            .HasColumnType("datetime2");
    }
}
