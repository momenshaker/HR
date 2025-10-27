using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class AnalyticsServiceTests
{
    private readonly Mock<IAnalyticsSnapshotRepository> _repositoryMock = new();
    private readonly AnalyticsService _sut;

    public AnalyticsServiceTests()
    {
        _sut = new AnalyticsService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsSnapshot()
    {
        // Arrange
        var request = new CreateAnalyticsSnapshotRequest
        {
            CapturedAtUtc = DateTime.UtcNow,
            Headcount = 100,
            TurnoverRate = 5,
            AverageTenureMonths = 24,
            HiringVelocity = 10,
            EngagementScore = 80,
            Commentary = "Stable"
        };

        AnalyticsSnapshot? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<AnalyticsSnapshot>(), It.IsAny<CancellationToken>()))
            .Callback<AnalyticsSnapshot, CancellationToken>((snapshot, _) => persisted = snapshot)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal(request.Headcount, persisted!.Headcount);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateAnalyticsSnapshotRequest
        {
            CapturedAtUtc = DateTime.UtcNow,
            Headcount = 100,
            TurnoverRate = 5,
            AverageTenureMonths = 24,
            HiringVelocity = 10,
            EngagementScore = 80,
            Commentary = "Stable"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AnalyticsSnapshot?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<AnalyticsSnapshot>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
