using System.Linq;
using System.Security.Cryptography;
using System.Text;
using HR.Application.Abstractions.Repositories;
using HR.Application.Common.Exceptions;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class LookupServiceTests
{
    private readonly Mock<ILookupRepository> _repositoryMock = new();
    private readonly LookupService _sut;

    public LookupServiceTests()
    {
        _sut = new LookupService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_GroupsAndSortsValues_ReturnsVersionToken()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var values = new[]
        {
            new LookupValue
            {
                Id = Guid.NewGuid(),
                Category = "region",
                Code = "EMEA",
                DisplayName = "EMEA",
                SortOrder = 2,
                IsActive = true,
                UpdatedAtUtc = now
            },
            new LookupValue
            {
                Id = Guid.NewGuid(),
                Category = "branch",
                Code = "HQ",
                DisplayName = "Headquarters",
                SortOrder = 3,
                IsActive = true,
                UpdatedAtUtc = now.AddMinutes(1)
            },
            new LookupValue
            {
                Id = Guid.NewGuid(),
                Category = "branch",
                Code = "FIELD",
                DisplayName = "Field",
                SortOrder = 1,
                IsActive = true,
                UpdatedAtUtc = now.AddMinutes(2)
            }
        };

        _repositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(values);

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        var orderedCategories = result.Categories.Select(c => c.Category).ToArray();
        Assert.Equal(new[] { "branch", "region" }, orderedCategories);
        var branchValues = result.Categories.First(c => c.Category == "branch").Values.ToArray();
        Assert.Collection(branchValues,
            value => Assert.Equal("Field", value.DisplayName),
            value => Assert.Equal("Headquarters", value.DisplayName));

        var expectedToken = ComputeExpectedToken(values);
        Assert.Equal(expectedToken, result.VersionToken);
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateExists_ThrowsUniqueConstraintViolationException()
    {
        // Arrange
        var request = new CreateLookupValueRequest
        {
            Category = "Branch",
            Code = "HQ",
            DisplayName = "Headquarters"
        };

        _repositoryMock
            .Setup(repo => repo.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act + Assert
        await Assert.ThrowsAsync<UniqueConstraintViolationException>(() => _sut.CreateAsync(request, CancellationToken.None));
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<LookupValue>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenSortOrderMissing_UsesNextSortOrder()
    {
        // Arrange
        var request = new CreateLookupValueRequest
        {
            Category = "Branch",
            Code = "nyc",
            DisplayName = "New York",
            SortOrder = null,
            IsActive = true
        };

        _repositoryMock
            .Setup(repo => repo.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock
            .Setup(repo => repo.GetNextSortOrderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);

        LookupValue? persisted = null;
        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<LookupValue>(), It.IsAny<CancellationToken>()))
            .Callback<LookupValue, CancellationToken>((value, _) => persisted = value)
            .ReturnsAsync((LookupValue value, CancellationToken _) => value);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal(7, persisted!.SortOrder);
        Assert.Equal("branch", persisted.Category);
        Assert.Equal("NYC", persisted.Code);
        Assert.Equal(7, result.SortOrder);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryAndCodeChange_EnsuresUniquenessAndNormalizesFields()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = new LookupValue
        {
            Id = id,
            Category = "branch",
            Code = "HQ",
            DisplayName = "Headquarters",
            SortOrder = 1,
            IsActive = true,
            UpdatedAtUtc = DateTime.UtcNow.AddDays(-1)
        };

        var request = new UpdateLookupValueRequest
        {
            Category = "Region",
            Code = "emea",
            DisplayName = "EMEA",
            SortOrder = null,
            IsActive = false
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repositoryMock
            .Setup(repo => repo.ExistsByCodeAsync("region", "EMEA", id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock
            .Setup(repo => repo.GetNextSortOrderAsync("region", It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        LookupValue? updated = null;
        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<LookupValue>(), It.IsAny<CancellationToken>()))
            .Callback<LookupValue, CancellationToken>((value, _) => updated = value)
            .ReturnsAsync((LookupValue value, CancellationToken _) => value);

        // Act
        var result = await _sut.UpdateAsync(id, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("region", updated!.Category);
        Assert.Equal("EMEA", updated.Code);
        Assert.Equal(5, updated.SortOrder);
        Assert.False(updated.IsActive);
        _repositoryMock.Verify(repo => repo.ExistsByCodeAsync("region", "EMEA", id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(repo => repo.GetNextSortOrderAsync("region", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryAndCodeUnchanged_DoesNotRecheckUniqueness()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = new LookupValue
        {
            Id = id,
            Category = "timeZone",
            Code = "UTC",
            DisplayName = "UTC",
            SortOrder = 3,
            IsActive = true,
            UpdatedAtUtc = DateTime.UtcNow.AddMinutes(-5)
        };

        var request = new UpdateLookupValueRequest
        {
            Category = existing.Category,
            Code = existing.Code,
            DisplayName = "Coordinated Universal Time",
            SortOrder = 4,
            IsActive = true
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<LookupValue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LookupValue value, CancellationToken _) => value);

        // Act
        var result = await _sut.UpdateAsync(id, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _repositoryMock.Verify(repo => repo.ExistsByCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(repo => repo.GetNextSortOrderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenValueDoesNotExist_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateLookupValueRequest
        {
            Category = "branch",
            Code = "HQ",
            DisplayName = "Headquarters"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LookupValue?)null);

        // Act
        var result = await _sut.UpdateAsync(id, request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<LookupValue>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static string ComputeExpectedToken(IEnumerable<LookupValue> values)
    {
        var total = 0L;
        var count = 0;
        foreach (var value in values)
        {
            total ^= value.UpdatedAtUtc.Ticks;
            count++;
        }

        var payload = $"{total}:{count}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash);
    }
}
