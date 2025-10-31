using FluentValidation;
using HR.Application.Abstractions.Repositories;
using HR.Application.Services;
using HR.Application.DTOs;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence.Repositories;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class TimesheetServiceTests
{
    private readonly InMemoryTimesheetRepository _timesheets = new();
    private readonly InMemoryEmployeeRepository _employees = new();
    private readonly InMemoryDepartmentRepository _departments = new();
    private readonly InMemoryEmployeeDepartmentRepository _employeeDepartments = new();

    private readonly TimesheetService _sut;
    private readonly Guid _employeeId = Guid.NewGuid();
    private readonly Guid _orgId = Guid.NewGuid();

    public TimesheetServiceTests()
    {
        _sut = new TimesheetService(_timesheets, _employees, _departments, _employeeDepartments);

        // Seed employee and department
        var deptId = Guid.NewGuid();
        _departments.AddAsync(new Department
        {
            Id = deptId,
            OrganizationId = _orgId,
            Name = "Engineering",
            Code = "ENG",
            Path = "/org/eng",
            Level = 0,
            CreatedAtUtc = DateTime.UtcNow
        }, CancellationToken.None).GetAwaiter().GetResult();

        _employees.AddAsync(new Employee
        {
            Id = _employeeId,
            Email = "employee@test.dev",
            FirstName = "Test",
            LastName = "User",
            Departments = new List<EmployeeDepartment> { new() { EmployeeId = _employeeId, DepartmentId = deptId, IsPrimary = true } }
        }, CancellationToken.None).GetAwaiter().GetResult();

        _employeeDepartments.AssignAsync(_employeeId, new[] { deptId }, CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Submit_ThenApprove_TransitionsSuccessfully()
    {
        var week = new DateOnly(2025, 10, 27);
        var ts = await _sut.GetWeekAsync(_employeeId, week, CancellationToken.None);

        var submitted = await _sut.SubmitAsync(ts.Id, CancellationToken.None);
        Assert.Equal(TimesheetStatus.Submitted, submitted!.Status);

        var approved = await _sut.ApproveAsync(ts.Id, Guid.NewGuid(), "OK", CancellationToken.None);
        Assert.Equal(TimesheetStatus.Approved, approved!.Status);
    }

    [Fact]
    public async Task UpsertEntry_EnforcesDailyHourCap()
    {
        var week = new DateOnly(2025, 10, 27);
        var ts = await _sut.GetWeekAsync(_employeeId, week, CancellationToken.None);

        // 20h first, ok
        await _sut.UpsertEntryAsync(ts.Id, new UpsertTimesheetEntryRequest
        {
            DateUtc = week,
            Hours = 20m
        }, CancellationToken.None);

        // adding 5h should fail (>24)
        await Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await _sut.UpsertEntryAsync(ts.Id, new UpsertTimesheetEntryRequest
            {
                DateUtc = week,
                Hours = 5m
            }, CancellationToken.None);
        });
    }

    [Fact]
    public async Task UpsertEntry_ValidatesDepartmentOrganization()
    {
        var otherOrgId = Guid.NewGuid();
        var foreignDeptId = Guid.NewGuid();

        await _departments.AddAsync(new Department
        {
            Id = foreignDeptId,
            OrganizationId = otherOrgId,
            Name = "Other",
            Code = "OTH",
            Path = "/org/oth",
            Level = 0,
            CreatedAtUtc = DateTime.UtcNow
        }, CancellationToken.None);

        var week = new DateOnly(2025, 10, 27);
        var ts = await _sut.GetWeekAsync(_employeeId, week, CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await _sut.UpsertEntryAsync(ts.Id, new UpsertTimesheetEntryRequest
            {
                DateUtc = week,
                Hours = 1m,
                DepartmentId = foreignDeptId
            }, CancellationToken.None);
        });
    }
}

