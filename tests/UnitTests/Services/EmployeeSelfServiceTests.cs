using System;
using System.Collections.Generic;
using System.Linq;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Services;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class EmployeeSelfServiceTests
{
    private readonly Mock<IAttendanceService> _attendanceServiceMock = new();
    private readonly Mock<ILeaveManagementService> _leaveServiceMock = new();
    private readonly Mock<IPayrollService> _payrollServiceMock = new();
    private readonly Mock<ITrainingService> _trainingServiceMock = new();
    private readonly EmployeeSelfService _sut;

    public EmployeeSelfServiceTests()
    {
        _sut = new EmployeeSelfService(
            _leaveServiceMock.Object,
            _attendanceServiceMock.Object,
            _payrollServiceMock.Object,
            _trainingServiceMock.Object);
    }

    [Fact]
    public async Task GetLeaveRequestsAsync_ReturnsOnlyEmployeeRecords()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var otherEmployeeId = Guid.NewGuid();
        var leaveRequests = new[]
        {
            new LeaveRequestDto(Guid.NewGuid(), employeeId, "Annual", DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), "Pending", null, string.Empty, DateTime.UtcNow, null),
            new LeaveRequestDto(Guid.NewGuid(), otherEmployeeId, "Sick", DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), "Approved", Guid.NewGuid(), string.Empty, DateTime.UtcNow, DateTime.UtcNow)
        };

        _leaveServiceMock
            .Setup(service => service.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(leaveRequests);

        // Act
        var result = await _sut.GetLeaveRequestsAsync(employeeId, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(employeeId, result.Single().EmployeeId);
    }

    [Fact]
    public async Task SubmitLeaveRequestAsync_WhenMismatchedEmployee_Throws()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = new CreateLeaveRequest
        {
            EmployeeId = Guid.NewGuid(),
            LeaveType = "Annual",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.SubmitLeaveRequestAsync(employeeId, request, CancellationToken.None));
    }

    [Fact]
    public async Task ClockInAsync_WhenOpenRecordExists_Throws()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var workDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var openRecord = new AttendanceRecordDto(
            Guid.NewGuid(),
            employeeId,
            workDate,
            "Morning",
            DateTime.UtcNow.AddHours(-2),
            null,
            0,
            "InProgress",
            string.Empty);

        _attendanceServiceMock
            .Setup(service => service.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { openRecord });

        var request = new ClockInRequest
        {
            TimestampUtc = DateTime.UtcNow,
            ShiftName = "Morning"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.ClockInAsync(employeeId, request, CancellationToken.None));
    }

    [Fact]
    public async Task ClockInAsync_PersistsAttendanceRecord()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        _attendanceServiceMock
            .Setup(service => service.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<AttendanceRecordDto>());

        var created = new AttendanceRecordDto(
            Guid.NewGuid(),
            employeeId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            "Default",
            DateTime.UtcNow,
            null,
            0,
            "InProgress",
            string.Empty);

        _attendanceServiceMock
            .Setup(service => service.CreateAsync(It.IsAny<CreateAttendanceRecordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var request = new ClockInRequest
        {
            TimestampUtc = DateTime.UtcNow
        };

        // Act
        var result = await _sut.ClockInAsync(employeeId, request, CancellationToken.None);

        // Assert
        Assert.Equal(created.Id, result.Id);
        _attendanceServiceMock.Verify(service =>
            service.CreateAsync(It.Is<CreateAttendanceRecordRequest>(payload => payload.EmployeeId == employeeId),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClockOutAsync_WhenRecordMissing_Throws()
    {
        // Arrange
        _attendanceServiceMock
            .Setup(service => service.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AttendanceRecordDto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.ClockOutAsync(Guid.NewGuid(), Guid.NewGuid(), new ClockOutRequest(), CancellationToken.None));
    }

    [Fact]
    public async Task ClockOutAsync_WhenRecordClosed_Throws()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var record = new AttendanceRecordDto(
            recordId,
            employeeId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            "Morning",
            DateTime.UtcNow.AddHours(-8),
            DateTime.UtcNow.AddHours(-1),
            0,
            "Completed",
            string.Empty);

        _attendanceServiceMock
            .Setup(service => service.GetByIdAsync(recordId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(record);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.ClockOutAsync(employeeId, recordId, new ClockOutRequest(), CancellationToken.None));
    }

    [Fact]
    public async Task GetSalarySlipsAsync_MapsPayrollRuns()
    {
        // Arrange
        var payrollRuns = new[]
        {
            new PayrollRunDto(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)), DateOnly.FromDateTime(DateTime.UtcNow), DateTime.UtcNow, "Completed", 10000m, 8000m, "Run"),
            new PayrollRunDto(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2)), DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)), DateTime.UtcNow.AddDays(-30), "Completed", 9500m, 7600m, "Run")
        };

        _payrollServiceMock
            .Setup(service => service.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(payrollRuns);

        // Act
        var result = await _sut.GetSalarySlipsAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, slip => Assert.Contains(slip.PayrollRunId, payrollRuns.Select(run => run.Id)));
    }
}
