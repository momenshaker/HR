using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder.ToTable("Candidates");

        builder.HasKey(candidate => candidate.Id);
        builder.Property(candidate => candidate.Id).ValueGeneratedNever();

        builder.Property(candidate => candidate.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(candidate => candidate.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(candidate => candidate.AppliedRole)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(candidate => candidate.Stage)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(candidate => candidate.Source)
            .HasMaxLength(100);

        builder.Property(candidate => candidate.AppliedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(candidate => candidate.NextInterviewAtUtc)
            .HasColumnType("datetime2");

        builder.Property(candidate => candidate.ResumeUrl)
            .HasMaxLength(512);

        builder.Property(candidate => candidate.Notes)
            .HasMaxLength(1024);

        builder.HasIndex(candidate => candidate.Email);
    }
}
