using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class EmployeeDepartmentConfiguration : IEntityTypeConfiguration<EmployeeDepartment>
{
    public void Configure(EntityTypeBuilder<EmployeeDepartment> builder)
    {
        builder.ToTable("EmployeeDepartments");

        builder.HasKey(membership => new { membership.EmployeeId, membership.DepartmentId });

        builder.Property(membership => membership.IsPrimary)
            .IsRequired();

        builder.HasIndex(membership => membership.EmployeeId);
        builder.HasIndex(membership => membership.DepartmentId);

        builder.HasOne(membership => membership.Employee)
            .WithMany(employee => employee.Departments)
            .HasForeignKey(membership => membership.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(membership => membership.Department)
            .WithMany(department => department.EmployeeDepartments)
            .HasForeignKey(membership => membership.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
