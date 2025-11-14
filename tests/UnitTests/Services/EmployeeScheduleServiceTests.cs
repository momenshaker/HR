using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class EmployeeScheduleServiceTests
{
    private readonly Mock<IEmployeeScheduleRepository> _repositoryMock = new();
    private readonly EmployeeScheduleService _sut;

    public EmployeeScheduleServiceTests()
    {
        _sut = new EmployeeScheduleService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsAssignment()
    {
        // Arrange
        var request = new CreateEmployeeScheduleRequest
        {
            EmployeeId = Guid.NewGuid(),
            WorkScheduleId = Guid.NewGuid(),
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        EmployeeSchedule? persisted = null;
        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<EmployeeSchedule>(), It.IsAny<CancellationToken>()))
            .Callback<EmployeeSchedule, CancellationToken>((schedule, _) => persisted = schedule)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.EmployeeId, result.EmployeeId);
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<EmployeeSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateEmployeeScheduleRequest
        {
            EmployeeId = Guid.NewGuid(),
            WorkScheduleId = Guid.NewGuid(),
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeSchedule?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<EmployeeSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
