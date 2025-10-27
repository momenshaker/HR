using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class TrainingServiceTests
{
    private readonly Mock<ITrainingCourseRepository> _repositoryMock = new();
    private readonly TrainingService _sut;

    public TrainingServiceTests()
    {
        _sut = new TrainingService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsCourse()
    {
        // Arrange
        var request = new CreateTrainingCourseRequest
        {
            Title = " Leadership 101 ",
            StartDate = new DateOnly(2025, 2, 1),
            EndDate = new DateOnly(2025, 2, 15),
            Capacity = 20
        };

        TrainingCourse? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<TrainingCourse>(), It.IsAny<CancellationToken>()))
            .Callback<TrainingCourse, CancellationToken>((course, _) => persisted = course)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Leadership 101", persisted!.Title);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateTrainingCourseRequest
        {
            Title = "Leadership 101",
            StartDate = new DateOnly(2025, 2, 1),
            EndDate = new DateOnly(2025, 2, 15),
            Capacity = 20
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingCourse?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<TrainingCourse>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
