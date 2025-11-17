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

        builder.HasIndex(employee => employee.Email).IsUnique();

        builder.Property(employee => employee.JobTitle)
            .HasMaxLength(150);

        builder.Property(employee => employee.PhoneNumber)
            .HasMaxLength(20)
            .HasDefaultValue(string.Empty)
            .IsRequired(false);

        builder.Property(employee => employee.EmploymentType)
            .HasMaxLength(40)
            .HasDefaultValue(string.Empty)
            .IsRequired(false);

        builder.Property(employee => employee.PaySchedule)
            .HasMaxLength(40)
            .HasDefaultValue("Monthly")
            .IsRequired(false);

        builder.Property(employee => employee.ContractType)
            .HasMaxLength(40)
            .HasDefaultValue(string.Empty)
            .IsRequired(false);

        builder.Property(employee => employee.PaymentMethod)
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty)
            .IsRequired(false);

        builder.Property(employee => employee.IBAN)
            .HasMaxLength(34)
            .HasDefaultValue(string.Empty)
            .IsRequired(false);

        builder.Property(employee => employee.BankName)
            .HasMaxLength(150)
            .HasDefaultValue(string.Empty)
            .IsRequired(false);

        builder.Property(employee => employee.BankAccountNumber)
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty)
            .IsRequired(false);

        builder.Property(employee => employee.EmploymentStartDate)
            .HasColumnType("date");

        builder.Property(employee => employee.EmploymentEndDate)
            .HasColumnType("date");

        builder.Property(employee => employee.DateOfBirth)
            .HasColumnType("date");

        builder.Property(employee => employee.CreatedAtUtc)
            .HasColumnType("datetime2");

        builder.ComplexProperty(employee => employee.JobArchitecture, architecture =>
        {
            architecture.Property(job => job.JobFamily)
                .HasColumnName("JobArchitectureJobFamily")
                .HasMaxLength(150)
                .HasDefaultValue("")
                .IsRequired(false);

            architecture.Property(job => job.JobFunction)
                .HasColumnName("JobArchitectureJobFunction")
                .HasMaxLength(150)
                .HasDefaultValue("")
                .IsRequired(false);

            architecture.Property(job => job.JobLevel)
                .HasColumnName("JobArchitectureJobLevel")
                .HasMaxLength(100)
                .HasDefaultValue("")
                .IsRequired(false);

            architecture.Property(job => job.JobCode)
                .HasColumnName("JobArchitectureJobCode")
                .HasMaxLength(100)
                .HasDefaultValue("")
                .IsRequired(false);

            architecture.Property(job => job.CareerTrack)
                .HasColumnName("JobArchitectureCareerTrack")
                .HasMaxLength(150)
                .HasDefaultValue("")
                .IsRequired(false);
        });

        builder.HasMany(employee => employee.Departments)
            .WithOne(employeeDepartment => employeeDepartment.Employee)
            .HasForeignKey(department => department.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(employee => employee.Departments).AutoInclude();

        builder.HasMany(employee => employee.ProfileDocuments)
            .WithOne(document => document.Employee)
            .HasForeignKey(document => document.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(employee => employee.ProfileDocuments).AutoInclude();

        builder.Ignore(employee => employee.FullName);
        builder.Ignore(employee => employee.DepartmentIds);
        builder.Ignore(employee => employee.PrimaryDepartmentId);
        builder.Ignore(employee => employee.SalaryStructure);
    }
}
