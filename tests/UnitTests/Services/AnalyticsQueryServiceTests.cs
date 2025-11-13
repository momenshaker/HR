using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence.EntityFramework;
using HR.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class AnalyticsQueryServiceTests
{
    private static HrDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<HrDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new HrDbContext(options);
    }

    [Fact]
    public async Task Headcount_Computes_By_Department_Subtree()
    {
        using var db = CreateDb();
        var orgId = Guid.NewGuid();
        var rootDept = new Department { Id = Guid.NewGuid(), OrganizationId = orgId, Name = "Root", Path = $"/org/{orgId}/dept/{Guid.NewGuid()}", IsActive = true };
        var subDept = new Department { Id = Guid.NewGuid(), OrganizationId = orgId, Name = "Sub", Path = $"{rootDept.Path}/{Guid.NewGuid()}", IsActive = true };
        db.Departments.AddRange(rootDept, subDept);

        var e1 = new Employee { Id = Guid.NewGuid(), FirstName = "A", LastName = "One", IsActive = true, EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)) };
        var e2 = new Employee { Id = Guid.NewGuid(), FirstName = "B", LastName = "Two", IsActive = true, EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)) };
        db.Employees.AddRange(e1, e2);
        db.EmployeeDepartments.AddRange(
            new EmployeeDepartment { EmployeeId = e1.Id, DepartmentId = rootDept.Id, IsPrimary = true },
            new EmployeeDepartment { EmployeeId = e2.Id, DepartmentId = subDept.Id, IsPrimary = true }
        );
        await db.SaveChangesAsync();

        var liteTraining = new Mock<ILightweightTrainingService>(MockBehavior.Strict);
        var sut = new EfAnalyticsQueryService(db, liteTraining.Object);

        var all = await sut.GetHeadcountAsync(orgId, null);
        Assert.Equal(2, all.Sum(i => i.Count));

        var subtree = await sut.GetHeadcountAsync(orgId, rootDept.Id);
        Assert.Equal(2, subtree.Sum(i => i.Count));
    }

    [Fact]
    public async Task LeaveUsage_Groups_By_Type_For_Year()
    {
        using var db = CreateDb();
        var orgId = Guid.NewGuid();
        var dept = new Department { Id = Guid.NewGuid(), OrganizationId = orgId, Name = "HR", Path = $"/org/{orgId}/dept/{Guid.NewGuid()}", IsActive = true };
        var emp = new Employee { Id = Guid.NewGuid(), FirstName = "L", LastName = "R", IsActive = true, EmploymentStartDate = new DateOnly(2022,1,1) };
        db.Departments.Add(dept); db.Employees.Add(emp);
        db.EmployeeDepartments.Add(new EmployeeDepartment { EmployeeId = emp.Id, DepartmentId = dept.Id, IsPrimary = true });

        db.LeaveRequests.AddRange(
            new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = emp.Id,
                LeaveTypeId = Guid.NewGuid(),
                LeaveType = "Vacation",
                Status = LeaveRequestStatus.Approved,
                StartDate = new DateOnly(2025, 1, 10),
                EndDate = new DateOnly(2025, 1, 12),
                NumberOfDays = 3m,
                SubmittedAtUtc = DateTime.UtcNow,
                ApprovedAtUtc = DateTime.UtcNow
            },
            new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = emp.Id,
                LeaveTypeId = Guid.NewGuid(),
                LeaveType = "Sick",
                Status = LeaveRequestStatus.Approved,
                StartDate = new DateOnly(2025, 2, 1),
                EndDate = new DateOnly(2025, 2, 1),
                NumberOfDays = 1m,
                SubmittedAtUtc = DateTime.UtcNow,
                ApprovedAtUtc = DateTime.UtcNow
            }
        );
        await db.SaveChangesAsync();

        var liteTraining = new Mock<ILightweightTrainingService>(MockBehavior.Strict);
        var sut = new EfAnalyticsQueryService(db, liteTraining.Object);
        var usage = await sut.GetLeaveUsageAsync(orgId, 2025);

        var vacation = usage.FirstOrDefault(x => x.LeaveType == "Vacation");
        Assert.NotNull(vacation);
        Assert.Equal(3, vacation!.Days);
        var sick = usage.FirstOrDefault(x => x.LeaveType == "Sick");
        Assert.NotNull(sick);
        Assert.Equal(1, sick!.Days);
    }

    [Fact]
    public async Task PayrollTotals_Summarizes_Runs_And_Departments()
    {
        using var db = CreateDb();
        var orgId = Guid.NewGuid();
        var dept = new Department { Id = Guid.NewGuid(), OrganizationId = orgId, Name = "Engineering", Path = $"/org/{orgId}/dept/{Guid.NewGuid()}", IsActive = true };
        var emp = new Employee { Id = Guid.NewGuid(), FirstName = "P", LastName = "E", IsActive = true, EmploymentStartDate = new DateOnly(2024,1,1) };
        db.Departments.Add(dept); db.Employees.Add(emp);
        db.EmployeeDepartments.Add(new EmployeeDepartment { EmployeeId = emp.Id, DepartmentId = dept.Id, IsPrimary = true });

        var run = new PayrollRun { Id = Guid.NewGuid(), OrganizationId = orgId, PeriodStart = new DateOnly(2025, 1, 1), PeriodEnd = new DateOnly(2025, 1, 31), TotalGrossPay = 1000, TotalNetPay = 800, Status = "Paid" };
        db.PayrollRuns.Add(run);
        db.PayrollItems.Add(new PayrollItem { Id = Guid.NewGuid(), RunId = run.Id, EmployeeId = emp.Id, Gross = 1000, Net = 800 });
        await db.SaveChangesAsync();

        var liteTraining = new Mock<ILightweightTrainingService>(MockBehavior.Strict);
        var sut = new EfAnalyticsQueryService(db, liteTraining.Object);
        var totals = await sut.GetPayrollTotalsAsync(orgId, new DateOnly(2025,1,1), new DateOnly(2025,2,1));

        Assert.Single(totals.Runs);
        Assert.Equal(1000, totals.Runs.First().TotalGross);
        Assert.Single(totals.ByDepartment);
        Assert.Equal(800, totals.ByDepartment.First().TotalNet);
    }

    [Fact]
    public async Task RecruitmentFunnel_Counts_By_Stage()
    {
        using var db = CreateDb();
        var vacancyId = Guid.NewGuid();
        db.InterviewSchedules.AddRange(
            new InterviewSchedule { Id = Guid.NewGuid(), VacancyId = vacancyId, CandidateId = Guid.NewGuid(), Stage = "Screen", ScheduledAtUtc = DateTime.UtcNow, Duration = TimeSpan.FromMinutes(30), Mode = "Phone" },
            new InterviewSchedule { Id = Guid.NewGuid(), VacancyId = vacancyId, CandidateId = Guid.NewGuid(), Stage = "Screen", ScheduledAtUtc = DateTime.UtcNow, Duration = TimeSpan.FromMinutes(30), Mode = "Phone" },
            new InterviewSchedule { Id = Guid.NewGuid(), VacancyId = vacancyId, CandidateId = Guid.NewGuid(), Stage = "Panel", ScheduledAtUtc = DateTime.UtcNow, Duration = TimeSpan.FromMinutes(60), Mode = "Video" }
        );
        await db.SaveChangesAsync();

        var liteTraining = new Mock<ILightweightTrainingService>(MockBehavior.Strict);
        var sut = new EfAnalyticsQueryService(db, liteTraining.Object);
        var funnel = await sut.GetRecruitmentFunnelAsync(vacancyId);

        var screen = funnel.FirstOrDefault(s => s.Stage == "Screen");
        Assert.NotNull(screen);
        Assert.Equal(2, screen!.Count);
        var panel = funnel.FirstOrDefault(s => s.Stage == "Panel");
        Assert.NotNull(panel);
        Assert.Equal(1, panel!.Count);
    }

    [Fact]
    public async Task TrainingCompliance_Computes_Rate_From_Gaps()
    {
        using var db = CreateDb();
        var orgId = Guid.NewGuid();
        var mockLite = new Mock<ILightweightTrainingService>();
        // Two observed employees: one compliant (0 gaps), one with 2 missing
        var gaps = new Dictionary<Guid, IReadOnlyCollection<Guid>>
        {
            [Guid.NewGuid()] = Array.Empty<Guid>(),
            [Guid.NewGuid()] = new [] { Guid.NewGuid(), Guid.NewGuid() }
        };
        mockLite.Setup(s => s.GetMandatoryCompletionGapsAsync(orgId, default)).ReturnsAsync(gaps);

        var sut = new EfAnalyticsQueryService(db, mockLite.Object);
        var tc = await sut.GetTrainingComplianceAsync(orgId);

        Assert.Equal(2, tc.ObservedEmployeeCount);
        Assert.Equal(1, tc.CompliantEmployeeCount);
        Assert.True(tc.ComplianceRate > 0 && tc.ComplianceRate < 1);
    }
}
