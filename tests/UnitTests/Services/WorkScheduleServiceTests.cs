using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class WorkScheduleServiceTests
{
    private readonly Mock<IWorkScheduleRepository> _repositoryMock = new();
    private readonly WorkScheduleService _sut;

    public WorkScheduleServiceTests()
    {
        _sut = new WorkScheduleService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsSchedule()
    {
        // Arrange
        var request = new CreateWorkScheduleRequest
        {
            Name = "Standard",
            ShiftTemplates = new[]
            {
                new ShiftTemplateRequest
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = TimeSpan.FromHours(9),
                    EndTime = TimeSpan.FromHours(18),
                    BreakMinutes = 60,
                    GracePeriodMinutes = 10,
                    MinimumOvertimeMinutes = 30
                }
            }
        };

        WorkSchedule? persisted = null;
        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<WorkSchedule>(), It.IsAny<CancellationToken>()))
            .Callback<WorkSchedule, CancellationToken>((schedule, _) => persisted = schedule)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal(request.Name, result.Name);
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<WorkSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateWorkScheduleRequest { Name = "Updated" };
        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkSchedule?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<WorkSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
