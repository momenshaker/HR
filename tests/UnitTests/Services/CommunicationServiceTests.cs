using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class CommunicationServiceTests
{
    private readonly Mock<IAnnouncementRepository> _announcementRepositoryMock = new();
    private readonly Mock<IEngagementCampaignRepository> _engagementCampaignRepositoryMock = new();
    private readonly Mock<IPulseSurveyRepository> _pulseSurveyRepositoryMock = new();
    private readonly Mock<IRecognitionProgramRepository> _recognitionProgramRepositoryMock = new();
    private readonly CommunicationService _sut;

    public CommunicationServiceTests()
    {
        _sut = new CommunicationService(
            _announcementRepositoryMock.Object,
            _engagementCampaignRepositoryMock.Object,
            _pulseSurveyRepositoryMock.Object,
            _recognitionProgramRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAnnouncementAsync_PersistsAnnouncement()
    {
        // Arrange
        var request = new CreateAnnouncementRequest
        {
            Title = " All Hands ",
            Message = "Quarterly update",
            CreatedBy = Guid.NewGuid()
        };

        Announcement? persisted = null;

        _announcementRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()))
            .Callback<Announcement, CancellationToken>((announcement, _) => persisted = announcement)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAnnouncementAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("All Hands", persisted!.Title);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAnnouncementAsync_WhenMissing_ReturnsNull()
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

        _announcementRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Announcement?)null);

        // Act
        var result = await _sut.UpdateAnnouncementAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _announcementRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateEngagementCampaignAsync_PersistsCampaign()
    {
        // Arrange
        var request = new CreateEngagementCampaignRequest
        {
            Name = " Culture Week ",
            Description = "Company-wide engagement",
            Channels = "email,app",
            TargetAudience = "Everyone",
            OwnerId = Guid.NewGuid(),
            LaunchDateUtc = DateTime.UtcNow.AddDays(1)
        };

        EngagementCampaign? persisted = null;

        _engagementCampaignRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<EngagementCampaign>(), It.IsAny<CancellationToken>()))
            .Callback<EngagementCampaign, CancellationToken>((campaign, _) => persisted = campaign)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateEngagementCampaignAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Culture Week", persisted!.Name);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task CreatePulseSurveyAsync_PersistsSurvey()
    {
        // Arrange
        var request = new CreatePulseSurveyRequest
        {
            Title = " Sentiment Check ",
            Description = "Quarterly pulse",
            Audience = "Managers",
            QuestionSet = "How satisfied are you?",
            ResponseWindowMinutes = 1440,
            OwnerId = Guid.NewGuid()
        };

        PulseSurvey? persisted = null;

        _pulseSurveyRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<PulseSurvey>(), It.IsAny<CancellationToken>()))
            .Callback<PulseSurvey, CancellationToken>((survey, _) => persisted = survey)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreatePulseSurveyAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Sentiment Check", persisted!.Title);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateRecognitionProgramAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateRecognitionProgramRequest
        {
            Name = "Spotlight",
            Description = "Celebrating values",
            Criteria = "Nomination",
            Reward = "Gift card",
            IsPeerToPeer = true,
            IsActive = true,
            OwnerId = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _recognitionProgramRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RecognitionProgram?)null);

        // Act
        var result = await _sut.UpdateRecognitionProgramAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _recognitionProgramRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<RecognitionProgram>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
