using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service responsible for internal communication operations.
/// </summary>
public interface ICommunicationService
{
    Task<IReadOnlyCollection<AnnouncementDto>> GetAnnouncementsAsync(CancellationToken cancellationToken = default);

    Task<AnnouncementDto?> GetAnnouncementByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AnnouncementDto> CreateAnnouncementAsync(CreateAnnouncementRequest request, CancellationToken cancellationToken = default);

    Task<AnnouncementDto?> UpdateAnnouncementAsync(Guid id, UpdateAnnouncementRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAnnouncementAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<EngagementCampaignDto>> GetEngagementCampaignsAsync(CancellationToken cancellationToken = default);

    Task<EngagementCampaignDto?> GetEngagementCampaignByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EngagementCampaignDto> CreateEngagementCampaignAsync(CreateEngagementCampaignRequest request, CancellationToken cancellationToken = default);

    Task<EngagementCampaignDto?> UpdateEngagementCampaignAsync(Guid id, UpdateEngagementCampaignRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteEngagementCampaignAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PulseSurveyDto>> GetPulseSurveysAsync(CancellationToken cancellationToken = default);

    Task<PulseSurveyDto?> GetPulseSurveyByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PulseSurveyDto> CreatePulseSurveyAsync(CreatePulseSurveyRequest request, CancellationToken cancellationToken = default);

    Task<PulseSurveyDto?> UpdatePulseSurveyAsync(Guid id, UpdatePulseSurveyRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeletePulseSurveyAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RecognitionProgramDto>> GetRecognitionProgramsAsync(CancellationToken cancellationToken = default);

    Task<RecognitionProgramDto?> GetRecognitionProgramByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RecognitionProgramDto> CreateRecognitionProgramAsync(CreateRecognitionProgramRequest request, CancellationToken cancellationToken = default);

    Task<RecognitionProgramDto?> UpdateRecognitionProgramAsync(Guid id, UpdateRecognitionProgramRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteRecognitionProgramAsync(Guid id, CancellationToken cancellationToken = default);
}
