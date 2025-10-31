using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(department => department.Id);
        builder.Property(department => department.Id).ValueGeneratedNever();

        builder.Property(department => department.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(department => department.Code)
            .HasMaxLength(50);

        builder.Property(department => department.Path)
            .IsRequired()
            .HasDefaultValue(string.Empty);

        builder.Property(department => department.Level)
            .HasDefaultValue(0);

        builder.Property(department => department.OrganizationId)
            .IsRequired();

        builder.Property(department => department.Branch)
            .HasMaxLength(100);

        builder.Property(department => department.Location)
            .HasMaxLength(200);

        builder.Property(department => department.Description)
            .HasMaxLength(1024);

        builder.Property(department => department.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(department => department.Organization)
            .WithMany(organization => organization.Departments)
            .HasForeignKey(department => department.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(department => department.Parent)
            .WithMany(parent => parent.Children)
            .HasForeignKey(department => department.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(department => new { department.OrganizationId, department.ParentDepartmentId, department.Name })
            .IsUnique();

        builder.HasIndex(department => new { department.OrganizationId, department.Code })
            .HasFilter($"{nameof(Department.Code)} IS NOT NULL")
            .IsUnique();
    }
}
