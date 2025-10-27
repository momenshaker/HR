using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(employee => employee.Id);
        builder.Property(employee => employee.Id).ValueGeneratedNever();

        builder.Property(employee => employee.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(employee => employee.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(employee => employee.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(employee => employee.JobTitle)
            .HasMaxLength(150);

        builder.Property(employee => employee.DepartmentId)
            .IsRequired();

        builder.Property(employee => employee.EmploymentStartDate)
            .HasColumnType("date");

        builder.Property(employee => employee.EmploymentEndDate)
            .HasColumnType("date");

        builder.Property(employee => employee.DateOfBirth)
            .HasColumnType("date");

        builder.Ignore(employee => employee.FullName);
    }
}
