using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;

namespace HR.UnitTests.Services;

public sealed class PerformanceManagementServiceTests
{
    private readonly Mock<IPerformanceReviewRepository> _repositoryMock = new();
    private readonly PerformanceManagementService _sut;

    public PerformanceManagementServiceTests()
    {
        _sut = new PerformanceManagementService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsReview()
    {
        // Arrange
        var request = new CreatePerformanceReviewRequest
        {
            EmployeeId = Guid.NewGuid(),
            CycleName = "2025 H1",
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 6, 30),
            OverallScore = 4,
            ManagerComments = "Great work"
        };

        PerformanceReview? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<PerformanceReview>(), It.IsAny<CancellationToken>()))
            .Callback<PerformanceReview, CancellationToken>((review, _) => persisted = review)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal(request.EmployeeId, persisted!.EmployeeId);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdatePerformanceReviewRequest
        {
            CycleName = "2025 H1",
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 6, 30),
            OverallScore = 4,
            ManagerComments = "Updated",
            GoalsSummary = string.Empty,
            SubmittedAtUtc = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PerformanceReview?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<PerformanceReview>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
