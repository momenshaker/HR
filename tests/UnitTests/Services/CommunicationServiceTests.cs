using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class CommunicationServiceTests
{
    private readonly Mock<IAnnouncementRepository> _repositoryMock = new();
    private readonly CommunicationService _sut;

    public CommunicationServiceTests()
    {
        _sut = new CommunicationService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsAnnouncement()
    {
        // Arrange
        var request = new CreateAnnouncementRequest
        {
            Title = " All Hands ",
            Message = "Quarterly update",
            CreatedBy = Guid.NewGuid()
        };

        Announcement? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()))
            .Callback<Announcement, CancellationToken>((announcement, _) => persisted = announcement)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("All Hands", persisted!.Title);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateAnnouncementRequest
        {
            Title = "All Hands",
            Message = "Quarterly update",
            Audience = string.Empty,
            CreatedBy = Guid.NewGuid(),
            RequiresAcknowledgement = false,
            PublishedAtUtc = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Announcement?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
