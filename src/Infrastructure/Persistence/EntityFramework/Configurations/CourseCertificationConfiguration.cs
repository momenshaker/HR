using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class CourseCertificationConfiguration : IEntityTypeConfiguration<CourseCertification>
{
    public void Configure(EntityTypeBuilder<CourseCertification> builder)
    {
        builder.ToTable("CourseCertifications");

        builder.HasKey(certification => certification.Id);
        builder.Property(certification => certification.Id).ValueGeneratedNever();

        builder.Property(certification => certification.CertificateNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(certification => certification.CertificateNumber).IsUnique();

        builder.Property(certification => certification.IssuedOn).HasColumnType("date");
        builder.Property(certification => certification.ExpiresOn).HasColumnType("date");

        builder.Property(certification => certification.IssuedBy)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(certification => certification.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(certification => certification.GovernanceNotes)
            .HasMaxLength(2000);
    }
}
