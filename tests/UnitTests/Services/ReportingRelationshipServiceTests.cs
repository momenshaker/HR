using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class ReportingRelationshipServiceTests
{
    private readonly Mock<IReportingRelationshipRepository> _repositoryMock = new();
    private readonly ReportingRelationshipService _sut;

    public ReportingRelationshipServiceTests()
    {
        _sut = new ReportingRelationshipService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsRelationship()
    {
        // Arrange
        ReportingRelationship? persisted = null;
        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<ReportingRelationship>(), It.IsAny<CancellationToken>()))
            .Callback<ReportingRelationship, CancellationToken>((entity, _) => persisted = entity)
            .ReturnsAsync(() => persisted!);

        var request = new CreateReportingRelationshipRequest
        {
            ManagerPositionId = Guid.NewGuid(),
            ReportPositionId = Guid.NewGuid(),
            RelationshipType = "Line",
            IsPrimary = true
        };

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal(request.ManagerPositionId, persisted!.ManagerPositionId);
        Assert.Equal(result.Id, persisted.Id);
    }
}
