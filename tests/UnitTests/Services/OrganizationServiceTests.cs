using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class OrganizationServiceTests
{
    private readonly Mock<IOrganizationRepository> _repositoryMock = new();
    private readonly OrganizationService _sut;

    public OrganizationServiceTests()
    {
        _sut = new OrganizationService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_MapsAndPersistsOrganization()
    {
        // Arrange
        var request = new CreateOrganizationRequest
        {
            Name = " Contoso HR ",
            Code = "con",
            Description = "Enterprise HR organization",
            IsActive = true
        };

        Organization? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
            .Callback<Organization, CancellationToken>((organization, _) => persisted = organization)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Contoso HR", persisted!.Name);
        Assert.Equal("CON", persisted.Code);
        Assert.Equal(request.Description.Trim(), persisted.Description);
        Assert.Equal(persisted.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrganizationMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateOrganizationRequest
        {
            Name = "Updated",
            Code = "UPD",
            Description = "Updated description",
            IsActive = false
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Organization?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
