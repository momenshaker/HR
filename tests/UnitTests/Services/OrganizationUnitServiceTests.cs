using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class OrganizationUnitServiceTests
{
    private readonly Mock<IOrganizationUnitRepository> _repositoryMock = new();
    private readonly OrganizationUnitService _sut;

    public OrganizationUnitServiceTests()
    {
        _sut = new OrganizationUnitService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetHierarchyAsync_BuildsNestedTree()
    {
        // Arrange
        var rootId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var units = new[]
        {
            new OrganizationUnit
            {
                Id = rootId,
                Name = "Headquarters",
                Code = "HQ",
                Type = "Division",
                Level = 0,
                Description = "",
                IsActive = true
            },
            new OrganizationUnit
            {
                Id = childId,
                Name = "Engineering",
                Code = "ENG",
                Type = "Department",
                Level = 1,
                ParentUnitId = rootId,
                Description = "",
                IsActive = true
            }
        };

        _repositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(units);

        // Act
        var hierarchy = await _sut.GetHierarchyAsync(CancellationToken.None);

        // Assert
        var root = Assert.Single(hierarchy);
        Assert.Equal(rootId, root.Unit.Id);
        var child = Assert.Single(root.Children);
        Assert.Equal(childId, child.Unit.Id);
    }

    [Fact]
    public async Task CreateAsync_NormalizesCode()
    {
        // Arrange
        OrganizationUnit? persisted = null;
        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<OrganizationUnit>(), It.IsAny<CancellationToken>()))
            .Callback<OrganizationUnit, CancellationToken>((unit, _) => persisted = unit)
            .ReturnsAsync(() => persisted!);

        var request = new CreateOrganizationUnitRequest
        {
            Name = " Engineering ",
            Code = "eng",
            Type = "Department",
            Level = 1
        };

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Engineering", persisted!.Name);
        Assert.Equal("ENG", persisted.Code);
        Assert.Equal(persisted.Id, result.Id);
    }
}
