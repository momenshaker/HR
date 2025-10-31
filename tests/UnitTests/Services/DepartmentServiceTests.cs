using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class DepartmentServiceTests
{
    private readonly Mock<IDepartmentRepository> _repositoryMock = new();
    private readonly DepartmentService _sut;
    private readonly Mock<IDepartmentTreeService> _treeServiceMock = new();
    private readonly Mock<IEmployeeDepartmentRepository> _employeeDepartmentRepositoryMock = new();

    public DepartmentServiceTests()
    {
        _sut = new DepartmentService(_repositoryMock.Object, _treeServiceMock.Object, _employeeDepartmentRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_MapsAndPersistsDepartment()
    {
        // Arrange
        var request = new CreateDepartmentRequest
        {
            Name = " People Ops ",
            Code = "hr",
            OrganizationId = Guid.NewGuid(),
            Branch = "HQ",
            Location = "Cairo"
        };

        Department? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
            .Callback<Department, CancellationToken>((department, _) => persisted = department)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request.OrganizationId, request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("People Ops", persisted!.Name);
        Assert.Equal("HR", persisted.Code);
        Assert.Equal(request.OrganizationId, persisted.OrganizationId);
        Assert.Equal(persisted.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenDepartmentMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateDepartmentRequest
        {
            Name = "Operations",
            Code = "OPS",
            OrganizationId = Guid.NewGuid()
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Department?)null);

        // Act
        var result = await _sut.UpdateAsync(request.OrganizationId, Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
