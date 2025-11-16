using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class HolidayServiceTests
{
    private readonly Mock<IHolidayRepository> _repositoryMock = new();
    private readonly HolidayService _sut;

    public HolidayServiceTests()
    {
        _sut = new HolidayService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsHoliday()
    {
        // Arrange
        var request = new CreateHolidayRequest
        {
            OrganizationId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Name = "New Year"
        };

        Holiday? persisted = null;
        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Holiday>(), It.IsAny<CancellationToken>()))
            .Callback<Holiday, CancellationToken>((holiday, _) => persisted = holiday)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Holiday>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateHolidayRequest
        {
            OrganizationId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Name = "Updated"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Holiday?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Holiday>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
