using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var secondaryDepartmentIdsComparer = new ValueComparer<IReadOnlyCollection<Guid>>(
            (left, right) => (left ?? Array.Empty<Guid>()).SequenceEqual(right ?? Array.Empty<Guid>()),
            collection => (collection ?? Array.Empty<Guid>()).Aggregate(0, (accumulator, guid) => HashCode.Combine(accumulator, guid.GetHashCode())),
            collection => (collection ?? Array.Empty<Guid>()).ToArray());

        var secondaryDepartmentIdsConverter = new ValueConverter<IReadOnlyCollection<Guid>, string>(
            ids => JsonSerializer.Serialize(ids ?? Array.Empty<Guid>(), jsonOptions),
            json => string.IsNullOrWhiteSpace(json)
                ? Array.Empty<Guid>()
                : JsonSerializer.Deserialize<List<Guid>>(json, jsonOptions) ?? new List<Guid>());

        builder.ComplexProperty(employee => employee.DepartmentAlignment, alignment =>
        {
            alignment.Property(department => department.PrimaryDepartmentId)
                .HasColumnName("DepartmentAlignmentPrimaryDepartmentId")
                .IsRequired();

            var secondaryDepartmentsProperty = alignment.Property(department => department.SecondaryDepartmentIds)
                .HasColumnName("DepartmentAlignmentSecondaryDepartmentIds")
                .HasConversion(secondaryDepartmentIdsConverter);

            secondaryDepartmentsProperty.Metadata.SetValueComparer(secondaryDepartmentIdsComparer);

            alignment.Property(department => department.ReportingDepartmentId)
                .HasColumnName("DepartmentAlignmentReportingDepartmentId");

            alignment.Property(department => department.CostCenter)
                .HasColumnName("DepartmentAlignmentCostCenter")
                .HasMaxLength(100);

            alignment.Property(department => department.BusinessUnit)
                .HasColumnName("DepartmentAlignmentBusinessUnit")
                .HasMaxLength(150);
        });

        builder.ComplexProperty(employee => employee.JobArchitecture, architecture =>
        {
            architecture.Property(job => job.JobFamily)
                .HasColumnName("JobArchitectureJobFamily")
                .HasMaxLength(150);

            architecture.Property(job => job.JobFunction)
                .HasColumnName("JobArchitectureJobFunction")
                .HasMaxLength(150);

            architecture.Property(job => job.JobLevel)
                .HasColumnName("JobArchitectureJobLevel")
                .HasMaxLength(100);

            architecture.Property(job => job.JobCode)
                .HasColumnName("JobArchitectureJobCode")
                .HasMaxLength(100);

            architecture.Property(job => job.CareerTrack)
                .HasColumnName("JobArchitectureCareerTrack")
                .HasMaxLength(150);
        });

        builder.Ignore(employee => employee.FullName);
    }
}
