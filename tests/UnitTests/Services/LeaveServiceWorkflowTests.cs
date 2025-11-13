using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence.Repositories;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class LeaveServiceWorkflowTests
{
    private readonly InMemoryLeaveRequestRepository _requestRepo = new();
    private readonly InMemoryLeaveTypeRepository _typeRepo = new();
    private readonly InMemoryLeaveBalanceRepository _balanceRepo = new();
    private readonly IWorkdayCalendar _calendar = new TestWorkdayCalendar();

    private readonly ILeaveService _sut;

    private readonly Guid _employee = Guid.NewGuid();
    private readonly Guid _manager = Guid.NewGuid();
    private readonly LeaveType _vacationType;

    public LeaveServiceWorkflowTests()
    {
        _sut = new LeaveService(_requestRepo, _typeRepo, _balanceRepo, _calendar);

        _vacationType = new LeaveType
        {
            Id = Guid.NewGuid(),
            Code = "VAC",
            Name = "Vacation",
            IsPaid = true,
            RequiresApproval = true,
            RequiresAttachment = false,
            AnnualAllowanceDays = 20,
            CarryOverDays = 5,
            Color = "#1976D2"
        };
        _typeRepo.AddAsync(_vacationType, default).GetAwaiter().GetResult();

        _balanceRepo.UpsertAsync(new LeaveBalance
        {
            EmployeeId = _employee,
            LeaveTypeId = _vacationType.Id,
            Year = DateTime.UtcNow.Year,
            Opening = 0,
            Accrued = 20,
            Taken = 0,
            CarriedOver = 0
        }, default).GetAwaiter().GetResult();
    }

    private sealed class TestWorkdayCalendar : IWorkdayCalendar
    {
        public bool IsWorkday(DateOnly date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }
    }

    [Fact]
    public async Task Preview_Computes_Business_Days()
    {
        // Given a Mon-Fri week
        var monday = GetNext(DayOfWeek.Monday);
        var friday = monday.AddDays(4);

        var preview = await _sut.PreviewAsync(_employee, _vacationType.Id, DateOnly.FromDateTime(monday), DateOnly.FromDateTime(friday));

        Assert.Equal(5m, preview.DurationDays);
        Assert.Equal(20m, preview.CurrentAvailable);
        Assert.Equal(0m, preview.Reserved);
        Assert.Equal(15m, preview.AvailableAfter);
    }

    [Fact]
    public async Task Submit_Pending_Reserves_Balance()
    {
        var start = GetNext(DayOfWeek.Monday);
        var end = start.AddDays(2); // 3 days

        var created = await _sut.SubmitAsync(new SubmitLeaveRequest
        {
            EmployeeId = _employee,
            LeaveTypeId = _vacationType.Id,
            StartDate = DateOnly.FromDateTime(start),
            EndDate = DateOnly.FromDateTime(end)
        });

        Assert.Equal(LeaveRequestStatus.PendingApproval, created.Status);

        var balances = await _sut.GetBalancesAsync(_employee, start.Year);
        var bal = Assert.Single(balances);
        Assert.Equal(3m, bal.Reserved);
        Assert.Equal(17m, bal.Remaining);
    }

    [Fact]
    public async Task Approve_Updates_Taken_And_Releases_Reserve()
    {
        var start = GetNext(DayOfWeek.Monday);
        var end = start.AddDays(1); // 2 days

        var created = await _sut.SubmitAsync(new SubmitLeaveRequest
        {
            EmployeeId = _employee,
            LeaveTypeId = _vacationType.Id,
            StartDate = DateOnly.FromDateTime(start),
            EndDate = DateOnly.FromDateTime(end)
        });

        var approved = await _sut.ApproveAsync(created.Id, _manager);
        Assert.Equal(LeaveRequestStatus.Approved, approved.Status);

        var balances = await _sut.GetBalancesAsync(_employee, start.Year);
        var bal = Assert.Single(balances);
        Assert.Equal(0m, bal.Reserved);
        Assert.Equal(18m, bal.Remaining); // 20 - taken(2)
    }

    [Fact]
    public async Task Reject_Releases_Hold()
    {
        var start = GetNext(DayOfWeek.Monday);
        var end = start.AddDays(0); // 1 day

        var created = await _sut.SubmitAsync(new SubmitLeaveRequest
        {
            EmployeeId = _employee,
            LeaveTypeId = _vacationType.Id,
            StartDate = DateOnly.FromDateTime(start),
            EndDate = DateOnly.FromDateTime(end)
        });

        var rejected = await _sut.RejectAsync(created.Id, _manager, "nope");
        Assert.Equal(LeaveRequestStatus.Rejected, rejected.Status);

        var balances = await _sut.GetBalancesAsync(_employee, start.Year);
        var bal = Assert.Single(balances);
        Assert.Equal(0m, bal.Reserved);
        Assert.Equal(20m, bal.Remaining);
    }

    [Fact]
    public async Task Cancel_Approved_Future_Reverts_Taken()
    {
        var start = GetNext(DayOfWeek.Monday).AddDays(14); // ensure future
        var end = start.AddDays(0); // 1 day

        var created = await _sut.SubmitAsync(new SubmitLeaveRequest
        {
            EmployeeId = _employee,
            LeaveTypeId = _vacationType.Id,
            StartDate = DateOnly.FromDateTime(start),
            EndDate = DateOnly.FromDateTime(end)
        });

        var approved = await _sut.ApproveAsync(created.Id, _manager);
        Assert.Equal(LeaveRequestStatus.Approved, approved.Status);

        var cancelled = await _sut.CancelAsync(created.Id, _employee);
        Assert.Equal(LeaveRequestStatus.Cancelled, cancelled.Status);

        var balances = await _sut.GetBalancesAsync(_employee, start.Year);
        var bal = Assert.Single(balances);
        Assert.Equal(20m, bal.Remaining);
        Assert.Equal(0m, bal.Reserved);
    }

    private static DateTime GetNext(DayOfWeek day)
    {
        var d = DateTime.UtcNow.Date;
        while (d.DayOfWeek != day)
            d = d.AddDays(1);
        return d;
    }
}
