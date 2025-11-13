using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Seeders;

/// <summary>
///     Provides canonical seed data for organization and department hierarchies used in tests and demos.
/// </summary>
public static class OrganizationStructureSeeder
{
    public static readonly Guid AcmeOrganizationId = new("8d741596-7f48-4a44-9a9b-5d4d78f3dc9a");
    public static readonly Guid HeadOfficeDepartmentId = new("5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90");
    public static readonly Guid EngineeringDepartmentId = new("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8");
    public static readonly Guid PlatformDepartmentId = new("3c7d4f9a-0f8b-4f33-9ed9-6bff8e75b5f2");
    public static readonly Guid ApplicationsDepartmentId = new("13d7a3fd-91c0-44d3-9d63-7f53820f9bde");
    public static readonly Guid HrDepartmentId = new("dc05d230-9a87-4c5c-a8cf-1e1422ecf7b2");

    public static readonly Guid AliceEmployeeId = new("df15d0d5-7b31-4a2c-924f-4da55b1fb677");
    public static readonly Guid BobEmployeeId = new("dca9c1f7-5fbc-4c6c-8fd0-9be0384fe760");
    public static readonly Guid CarolEmployeeId = new("c0d7683e-8e3d-4f6d-bf16-b0cb02f1455e");
    public static readonly Guid DavidEmployeeId = new("0b90b2f9-2378-4e8f-9b77-4d56e0dcd8ec");

    private static readonly DateTime SeedCreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateOnly SeedStartDate = new(2020, 1, 6);

    public static void Seed(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        var rootPath = $"/org/{AcmeOrganizationId}/dept";
        var headOfficePath = $"{rootPath}/{HeadOfficeDepartmentId}";
        var engineeringPath = $"{headOfficePath}/{EngineeringDepartmentId}";
        var platformPath = $"{engineeringPath}/{PlatformDepartmentId}";
        var applicationsPath = $"{engineeringPath}/{ApplicationsDepartmentId}";
        var hrPath = $"{headOfficePath}/{HrDepartmentId}";

        modelBuilder.Entity<Organization>().HasData(new Organization
        {
            Id = AcmeOrganizationId,
            Name = "Acme Corp",
            Code = "ACME",
            Description = "Acme Corp demo organization",
            Industry = "Professional Services",
            Region = "North America",
            HeadquartersAddress = "1 Summit Avenue, New York, NY 10004",
            TimeZone = "America/New_York",
            PrimaryContactEmail = "hello@acme.test",
            WebsiteUrl = "https://acme.test",
            IsActive = true,
            CreatedAtUtc = SeedCreatedAt
        });

        modelBuilder.Entity<Department>().HasData(
            new Department
            {
                Id = HeadOfficeDepartmentId,
                OrganizationId = AcmeOrganizationId,
                Name = "Head Office",
                Code = "HQ",
                Path = headOfficePath,
                Level = 0,
                Branch = "Global",
                Location = "New York",
                BusinessUnit = "Corporate",
                CostCenterCode = "CC-HQ-001",
                OperatingHours = "Mon-Fri 08:00-18:00",
                BudgetOwner = "Executive Finance",
                Description = "Corporate headquarters",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAt
            },
            new Department
            {
                Id = EngineeringDepartmentId,
                OrganizationId = AcmeOrganizationId,
                ParentDepartmentId = HeadOfficeDepartmentId,
                Name = "Engineering",
                Code = "ENG",
                Path = engineeringPath,
                Level = 1,
                Branch = "Product",
                Location = "Seattle",
                BusinessUnit = "Engineering",
                CostCenterCode = "CC-ENG-200",
                OperatingHours = "Mon-Fri 08:00-18:00",
                BudgetOwner = "VP Engineering",
                Description = "Platform and application engineering",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAt
            },
            new Department
            {
                Id = PlatformDepartmentId,
                OrganizationId = AcmeOrganizationId,
                ParentDepartmentId = EngineeringDepartmentId,
                Name = "Platform",
                Code = "PLAT",
                Path = platformPath,
                Level = 2,
                Branch = "Technology",
                Location = "Seattle",
                BusinessUnit = "Platform Services",
                CostCenterCode = "CC-ENG-210",
                OperatingHours = "24/7 (rotating)",
                BudgetOwner = "Platform Director",
                Description = "Platform services and infrastructure",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAt
            },
            new Department
            {
                Id = ApplicationsDepartmentId,
                OrganizationId = AcmeOrganizationId,
                ParentDepartmentId = EngineeringDepartmentId,
                Name = "Applications",
                Code = "APPS",
                Path = applicationsPath,
                Level = 2,
                Branch = "Product",
                Location = "Austin",
                BusinessUnit = "Customer Experience",
                CostCenterCode = "CC-ENG-220",
                OperatingHours = "Mon-Fri 09:00-17:30",
                BudgetOwner = "Applications Lead",
                Description = "Customer-facing application development",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAt
            },
            new Department
            {
                Id = HrDepartmentId,
                OrganizationId = AcmeOrganizationId,
                ParentDepartmentId = HeadOfficeDepartmentId,
                Name = "HR",
                Code = "HR",
                Path = hrPath,
                Level = 1,
                Branch = "Corporate",
                Location = "New York",
                BusinessUnit = "People",
                CostCenterCode = "CC-HR-300",
                OperatingHours = "Mon-Fri 08:00-17:00",
                BudgetOwner = "Chief People Officer",
                Description = "People operations",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAt
            }
        );

        // Use anonymous objects to avoid setting complex/owned properties (e.g., JobArchitecture)
        modelBuilder.Entity<Employee>().HasData(
            new
            {
                Id = AliceEmployeeId,
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@acme.test",
                JobTitle = "Director of Engineering",
                EmploymentStartDate = SeedStartDate,
                CreatedAtUtc = SeedCreatedAt,
                IsActive = true
            },
            new
            {
                Id = BobEmployeeId,
                FirstName = "Bob",
                LastName = "Smith",
                Email = "bob.smith@acme.test",
                JobTitle = "Senior Platform Engineer",
                EmploymentStartDate = SeedStartDate.AddMonths(3),
                CreatedAtUtc = SeedCreatedAt,
                IsActive = true
            },
            new
            {
                Id = CarolEmployeeId,
                FirstName = "Carol",
                LastName = "Lee",
                Email = "carol.lee@acme.test",
                JobTitle = "Applications Lead",
                EmploymentStartDate = SeedStartDate.AddMonths(6),
                CreatedAtUtc = SeedCreatedAt,
                IsActive = true
            },
            new
            {
                Id = DavidEmployeeId,
                FirstName = "David",
                LastName = "Patel",
                Email = "david.patel@acme.test",
                JobTitle = "HR Business Partner",
                EmploymentStartDate = SeedStartDate.AddMonths(2),
                CreatedAtUtc = SeedCreatedAt,
                IsActive = true
            }
        );

        modelBuilder.Entity<EmployeeDepartment>().HasData(
            new EmployeeDepartment
            {
                EmployeeId = AliceEmployeeId,
                DepartmentId = EngineeringDepartmentId,
                IsPrimary = true
            },
            new EmployeeDepartment
            {
                EmployeeId = AliceEmployeeId,
                DepartmentId = PlatformDepartmentId,
                IsPrimary = false
            },
            new EmployeeDepartment
            {
                EmployeeId = BobEmployeeId,
                DepartmentId = PlatformDepartmentId,
                IsPrimary = true
            },
            new EmployeeDepartment
            {
                EmployeeId = BobEmployeeId,
                DepartmentId = ApplicationsDepartmentId,
                IsPrimary = false
            },
            new EmployeeDepartment
            {
                EmployeeId = CarolEmployeeId,
                DepartmentId = ApplicationsDepartmentId,
                IsPrimary = true
            },
            new EmployeeDepartment
            {
                EmployeeId = CarolEmployeeId,
                DepartmentId = EngineeringDepartmentId,
                IsPrimary = false
            },
            new EmployeeDepartment
            {
                EmployeeId = DavidEmployeeId,
                DepartmentId = HrDepartmentId,
                IsPrimary = true
            },
            new EmployeeDepartment
            {
                EmployeeId = DavidEmployeeId,
                DepartmentId = HeadOfficeDepartmentId,
                IsPrimary = false
            }
        );
    }
}
