using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class RecruitmentServiceTests
{
    private readonly Mock<ICandidateRepository> _repositoryMock = new();
    private readonly RecruitmentService _sut;

    public RecruitmentServiceTests()
    {
        _sut = new RecruitmentService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsCandidate()
    {
        // Arrange
        var request = new CreateCandidateRequest
        {
            FullName = " Jane Doe ",
            Email = "jane.doe@example.com",
            AppliedRole = "Engineer",
            Stage = "Applied"
        };

        Candidate? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Candidate>(), It.IsAny<CancellationToken>()))
            .Callback<Candidate, CancellationToken>((candidate, _) => persisted = candidate)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Jane Doe", persisted!.FullName);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateCandidateRequest
        {
            FullName = "Jane Doe",
            Email = "jane.doe@example.com",
            AppliedRole = "Engineer"
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Candidate?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Candidate>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
