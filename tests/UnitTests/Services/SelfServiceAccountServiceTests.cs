using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class SelfServiceAccountServiceTests
{
    private readonly Mock<ISelfServiceAccountRepository> _repositoryMock = new();
    private readonly SelfServiceAccountService _sut;

    public SelfServiceAccountServiceTests()
    {
        _sut = new SelfServiceAccountService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenAccountExists_Throws()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        _repositoryMock
            .Setup(repo => repo.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SelfServiceAccount { Id = Guid.NewGuid(), EmployeeId = employeeId });

        var request = new CreateSelfServiceAccountRequest
        {
            EmployeeId = employeeId,
            Email = "user@example.com",
            OAuthProvider = "AzureAD",
            ExternalIdentifier = "sub-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_NormalizesAccountDetails()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        SelfServiceAccount? persisted = null;

        _repositoryMock
            .Setup(repo => repo.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SelfServiceAccount?)null);

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<SelfServiceAccount>(), It.IsAny<CancellationToken>()))
            .Callback<SelfServiceAccount, CancellationToken>((account, _) => persisted = account)
            .ReturnsAsync(() => persisted!);

        var request = new CreateSelfServiceAccountRequest
        {
            EmployeeId = employeeId,
            Email = " User@Example.com ",
            OAuthProvider = " AzureAD ",
            ExternalIdentifier = " sub-123 ",
            FeatureAccess = new[] { "  Leave ", "PAYROLL", "leave" }
        };

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal(employeeId, persisted!.EmployeeId);
        Assert.Equal("user@example.com", persisted.Email);
        Assert.Equal("AzureAD", persisted.OAuthProvider);
        Assert.Equal("sub-123", persisted.ExternalIdentifier);
        Assert.Equal(new[] { "Leave", "PAYROLL" }, persisted.FeatureAccess);
        Assert.Equal(persisted.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenAccountMissing_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SelfServiceAccount?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateSelfServiceAccountRequest(), CancellationToken.None);

        Assert.Null(result);
    }
}
