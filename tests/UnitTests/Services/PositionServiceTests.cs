using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class PositionServiceTests
{
    private readonly Mock<IPositionRepository> _repositoryMock = new();
    private readonly PositionService _sut;

    public PositionServiceTests()
    {
        _sut = new PositionService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByEmployeeIdAsync_ReturnsDto()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var position = new Position
        {
            Id = Guid.NewGuid(),
            Title = "Engineering Manager",
            JobCode = "ENGMGR",
            OrganizationUnitId = Guid.NewGuid(),
            OccupiedByEmployeeId = employeeId,
            EmploymentType = "FullTime"
        };

        _repositoryMock
            .Setup(repo => repo.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);

        // Act
        var result = await _sut.GetByEmployeeIdAsync(employeeId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(position.Id, result!.Id);
    }

    [Fact]
    public async Task CreateAsync_NormalizesFields()
    {
        // Arrange
        Position? persisted = null;
        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
            .Callback<Position, CancellationToken>((entity, _) => persisted = entity)
            .ReturnsAsync(() => persisted!);

        var request = new CreatePositionRequest
        {
            Title = " engineering manager ",
            JobCode = "engmgr",
            OrganizationUnitId = Guid.NewGuid(),
            Grade = "L2"
        };

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("engineering manager", persisted!.Title);
        Assert.Equal("ENGMGR", persisted.JobCode);
        Assert.Equal(persisted.Id, result.Id);
    }
}
