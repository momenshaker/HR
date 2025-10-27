using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class LeaveManagementServiceTests
{
    private readonly Mock<ILeaveRequestRepository> _repositoryMock = new();
    private readonly LeaveManagementService _sut;

    public LeaveManagementServiceTests()
    {
        _sut = new LeaveManagementService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_SetsPendingStatus()
    {
        // Arrange
        var request = new CreateLeaveRequest
        {
            EmployeeId = Guid.NewGuid(),
            LeaveType = "Vacation",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Reason = "Family trip"
        };

        LeaveRequest? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()))
            .Callback<LeaveRequest, CancellationToken>((leave, _) => persisted = leave)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Pending", persisted!.Status);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateLeaveRequest
        {
            LeaveType = "Sick",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LeaveRequest?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
