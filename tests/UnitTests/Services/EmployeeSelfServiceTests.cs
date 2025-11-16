using System;
using System.Collections.Generic;
using System.Linq;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class EmployeeSelfServiceTests
{
    private readonly Mock<IAttendanceService> _attendanceServiceMock = new();
    private readonly Mock<ILeaveManagementService> _leaveServiceMock = new();
    private readonly Mock<IPayrollService> _payrollServiceMock = new();
    private readonly Mock<ITrainingService> _trainingServiceMock = new();
    private readonly Mock<IPositionService> _positionServiceMock = new();
    private readonly Mock<IOrganizationUnitService> _organizationUnitServiceMock = new();
    private readonly Mock<IReportingRelationshipService> _reportingRelationshipServiceMock = new();
    private readonly Mock<IDelegatedAuthorityService> _delegatedAuthorityServiceMock = new();
    private readonly Mock<ISelfServiceAccountService> _selfServiceAccountServiceMock = new();
    private readonly EmployeeSelfService _sut;

    public EmployeeSelfServiceTests()
    {
        _sut = new EmployeeSelfService(
            _leaveServiceMock.Object,
            _attendanceServiceMock.Object,
            _payrollServiceMock.Object,
            _trainingServiceMock.Object,
            _positionServiceMock.Object,
            _organizationUnitServiceMock.Object,
            _reportingRelationshipServiceMock.Object,
            _delegatedAuthorityServiceMock.Object,
            _selfServiceAccountServiceMock.Object);
    }

    [Fact]
    public async Task GetLeaveRequestsAsync_ReturnsOnlyEmployeeRecords()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var otherEmployeeId = Guid.NewGuid();
        var leaveRequests = new[]
        {
            new LeaveRequestDto(
                Guid.NewGuid(),
                employeeId,
                Guid.NewGuid(),
                "Annual",
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                2m,
                LeaveRequestStatus.PendingApproval,
                null,
                string.Empty,
                null,
                DateTime.UtcNow,
                null,
                null,
                null),
            new LeaveRequestDto(
                Guid.NewGuid(),
                otherEmployeeId,
                Guid.NewGuid(),
                "Sick",
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                1m,
                LeaveRequestStatus.Approved,
                Guid.NewGuid(),
                string.Empty,
                null,
                DateTime.UtcNow,
                DateTime.UtcNow,
                null,
                null)
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
            LeaveTypeId = Guid.NewGuid(),
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
        var openRecord = CreateAttendanceRecordDto(
            Guid.NewGuid(),
            employeeId,
            workDate,
            "Morning",
            "InProgress",
            Array.Empty<AttendancePunchDto>());

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

        var created = CreateAttendanceRecordDto(
            Guid.NewGuid(),
            employeeId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            "Default",
            "InProgress",
            Array.Empty<AttendancePunchDto>());

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
        var record = CreateAttendanceRecordDto(
            recordId,
            employeeId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            "Morning",
            "Completed",
            new[]
            {
                new AttendancePunchDto(Guid.NewGuid(), "ClockIn", DateTimeOffset.UtcNow.AddHours(-8), "SelfService", string.Empty, string.Empty, string.Empty),
                new AttendancePunchDto(Guid.NewGuid(), "ClockOut", DateTimeOffset.UtcNow.AddHours(-1), "SelfService", string.Empty, string.Empty, string.Empty)
            });

        _attendanceServiceMock
            .Setup(service => service.GetByIdAsync(recordId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(record);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.ClockOutAsync(employeeId, recordId, new ClockOutRequest(), CancellationToken.None));
    }

    [Fact]
    public async Task GetSalarySlipsAsync_ReturnsValuesFromPayrollService()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var salarySlips = new[]
        {
            new SalarySlipDto(Guid.NewGuid(), employeeId, DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)), DateTime.UtcNow.AddDays(-10), "Completed", 5000m, 4200m, "Processed"),
            new SalarySlipDto(Guid.NewGuid(), employeeId, DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2)), DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2).AddDays(15)), DateTime.UtcNow.AddDays(-40), "Completed", 5100m, 4300m, "Processed")
        };

        _payrollServiceMock
            .Setup(service => service.GetSalarySlipsAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(salarySlips);

        // Act
        var result = await _sut.GetSalarySlipsAsync(employeeId, CancellationToken.None);

        // Assert
        Assert.Equal(salarySlips, result);
        _payrollServiceMock.Verify(service => service.GetSalarySlipsAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrganizationSnapshotAsync_ComposesHierarchyAndDelegations()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var positionId = Guid.NewGuid();
        var organizationUnitId = Guid.NewGuid();

        var position = new PositionDto(positionId, "Head of Engineering", "ENG001", organizationUnitId, null, employeeId, "L2", "FullTime", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)), null, true, false);
        var organizationUnit = new OrganizationUnitDto(organizationUnitId, "Engineering", "ENG", "Division", null, null, null, 1, "", true);
        var reportingRelationship = new ReportingRelationshipDto(Guid.NewGuid(), positionId, Guid.NewGuid(), "Line", DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)), null, true);
        var delegatedAuthority = new DelegatedAuthorityDto(Guid.NewGuid(), employeeId, employeeId, null, null, "Approve CapEx", 5000m, DateTimeOffset.UtcNow.AddDays(-10), DateTimeOffset.UtcNow.AddDays(20), null, false, string.Empty);
        var account = new SelfServiceAccountDto(Guid.NewGuid(), employeeId, "user@example.com", "AzureAD", "sub-123", true, false, DateTimeOffset.UtcNow.AddMonths(-3), DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow, new[] { "Leave", "Payroll" });

        _positionServiceMock
            .Setup(service => service.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);

        _organizationUnitServiceMock
            .Setup(service => service.GetByIdAsync(organizationUnitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(organizationUnit);

        _reportingRelationshipServiceMock
            .Setup(service => service.GetByReportPositionAsync(positionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { reportingRelationship });

        _reportingRelationshipServiceMock
            .Setup(service => service.GetByManagerPositionAsync(positionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ReportingRelationshipDto>());

        _delegatedAuthorityServiceMock
            .Setup(service => service.GetByDelegateAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { delegatedAuthority });

        _selfServiceAccountServiceMock
            .Setup(service => service.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var snapshot = await _sut.GetOrganizationSnapshotAsync(employeeId, CancellationToken.None);

        // Assert
        Assert.Equal(employeeId, snapshot.EmployeeId);
        Assert.Equal(positionId, snapshot.Position?.Id);
        Assert.Equal(organizationUnitId, snapshot.OrganizationUnit?.Id);
        Assert.Single(snapshot.ReportingLines);
        Assert.Single(snapshot.DelegatedAuthorities);
        Assert.Equal(account.Id, snapshot.SelfServiceAccount?.Id);
    }

    [Fact]
    public async Task RegisterAccountAsync_PassesThroughToService()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = new CreateSelfServiceAccountRequest
        {
            EmployeeId = employeeId,
            Email = "user@example.com",
            OAuthProvider = "AzureAD",
            ExternalIdentifier = "sub-123",
            FeatureAccess = new[] { "Leave" }
        };

        var account = new SelfServiceAccountDto(Guid.NewGuid(), employeeId, request.Email, request.OAuthProvider, request.ExternalIdentifier, true, false, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, null, new[] { "Leave" });

        _selfServiceAccountServiceMock
            .Setup(service => service.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _sut.RegisterAccountAsync(employeeId, request, CancellationToken.None);

        // Assert
        Assert.Equal(account.Id, result.Id);
        _selfServiceAccountServiceMock.Verify(service => service.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAccountAsync_WhenAccountMissing_ReturnsNull()
    {
        // Arrange
        _selfServiceAccountServiceMock
            .Setup(service => service.GetByEmployeeIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SelfServiceAccountDto?)null);

        // Act
        var result = await _sut.UpdateAccountAsync(Guid.NewGuid(), new UpdateSelfServiceAccountRequest(), CancellationToken.None);

        // Assert
        Assert.Null(result);
        _selfServiceAccountServiceMock.Verify(service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateSelfServiceAccountRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static AttendanceRecordDto CreateAttendanceRecordDto(
        Guid id,
        Guid employeeId,
        DateOnly workDate,
        string shiftName,
        string status,
        IReadOnlyCollection<AttendancePunchDto> punches)
    {
        return new AttendanceRecordDto(
            id,
            employeeId,
            workDate,
            shiftName,
            null,
            null,
            null,
            null,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            status,
            "Manual",
            string.Empty,
            punches);
    }
}
