using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using System;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class AttendanceServiceTests
{
    private readonly Mock<IAttendanceRecordRepository> _repositoryMock = new();
    private readonly AttendanceService _sut;

    public AttendanceServiceTests()
    {
        _sut = new AttendanceService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsRecord()
    {
        // Arrange
        var request = new CreateAttendanceRecordRequest
        {
            EmployeeId = Guid.NewGuid(),
            WorkDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ShiftName = "Morning",
            Punches = new[]
            {
                new AttendancePunchRequest
                {
                    Type = "ClockIn",
                    TimestampUtc = DateTimeOffset.UtcNow
                }
            }
        };

        AttendanceRecord? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<AttendanceRecord>(), It.IsAny<CancellationToken>()))
            .Callback<AttendanceRecord, CancellationToken>((record, _) => persisted = record)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal(request.EmployeeId, persisted!.EmployeeId);
        Assert.Equal(persisted.Id, result.Id);
        Assert.Single(persisted.Punches);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateAttendanceRecordRequest
        {
            EmployeeId = Guid.NewGuid(),
            WorkDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AttendanceRecord?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<AttendanceRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
