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

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(membership => membership.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
