using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class DelegatedAuthorityServiceTests
{
    private readonly Mock<IDelegatedAuthorityRepository> _repositoryMock = new();
    private readonly DelegatedAuthorityService _sut;

    public DelegatedAuthorityServiceTests()
    {
        _sut = new DelegatedAuthorityService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByDelegateAsync_ReturnsMappedResults()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var authorities = new[]
        {
            new DelegatedAuthority
            {
                Id = Guid.NewGuid(),
                DelegateEmployeeId = employeeId,
                AuthorityScope = "Approve Expenses",
                GrantedOnUtc = DateTimeOffset.UtcNow
            }
        };

        _repositoryMock
            .Setup(repo => repo.GetByDelegateAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorities);

        // Act
        var result = await _sut.GetByDelegateAsync(employeeId, CancellationToken.None);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(authorities[0].Id, dto.Id);
    }
}
