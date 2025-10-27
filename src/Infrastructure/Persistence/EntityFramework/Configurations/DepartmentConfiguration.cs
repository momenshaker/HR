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
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(department => department.Branch)
            .HasMaxLength(100);

        builder.Property(department => department.Location)
            .HasMaxLength(200);

        builder.Property(department => department.Description)
            .HasMaxLength(1024);

        builder.Property(department => department.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(department => department.Code).IsUnique();
    }
}
