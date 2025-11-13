using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class EmployeeProfileDocumentConfiguration : IEntityTypeConfiguration<EmployeeProfileDocument>
{
    public void Configure(EntityTypeBuilder<EmployeeProfileDocument> builder)
    {
        builder.ToTable("EmployeeProfileDocuments");

        builder.HasKey(document => document.Id);
        builder.Property(document => document.Id).ValueGeneratedNever();

        builder.Property(document => document.FileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(document => document.Description)
            .IsRequired()
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty);

        builder.Property(document => document.StoragePath)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(document => document.ContentType)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue(string.Empty);

        builder.Property(document => document.UploadedAtUtc)
            .HasColumnType("datetimeoffset");

        builder.HasOne(document => document.Employee)
            .WithMany(employee => employee.ProfileDocuments)
            .HasForeignKey(document => document.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
