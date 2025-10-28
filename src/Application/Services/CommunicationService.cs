using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class CommunicationService : ICommunicationService
{
    private readonly IAnnouncementRepository _announcementRepository;
    private readonly IEngagementCampaignRepository _engagementCampaignRepository;
    private readonly IPulseSurveyRepository _pulseSurveyRepository;
    private readonly IRecognitionProgramRepository _recognitionProgramRepository;

    public CommunicationService(
        IAnnouncementRepository announcementRepository,
        IEngagementCampaignRepository engagementCampaignRepository,
        IPulseSurveyRepository pulseSurveyRepository,
        IRecognitionProgramRepository recognitionProgramRepository)
    {
        _announcementRepository = announcementRepository;
        _engagementCampaignRepository = engagementCampaignRepository;
        _pulseSurveyRepository = pulseSurveyRepository;
        _recognitionProgramRepository = recognitionProgramRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AnnouncementDto>> GetAnnouncementsAsync(CancellationToken cancellationToken = default)
    {
        var announcements = await _announcementRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return announcements.Select(announcement => announcement.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<AnnouncementDto?> GetAnnouncementByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var announcement = await _announcementRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return announcement?.ToDto();
    }

    /// <inheritdoc />
    public async Task<AnnouncementDto> CreateAnnouncementAsync(CreateAnnouncementRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _announcementRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<AnnouncementDto?> UpdateAnnouncementAsync(Guid id, UpdateAnnouncementRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _announcementRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _announcementRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAnnouncementAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _announcementRepository.RemoveAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<EngagementCampaignDto>> GetEngagementCampaignsAsync(CancellationToken cancellationToken = default)
    {
        var campaigns = await _engagementCampaignRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return campaigns.Select(campaign => campaign.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<EngagementCampaignDto?> GetEngagementCampaignByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await _engagementCampaignRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return campaign?.ToDto();
    }

    /// <inheritdoc />
    public async Task<EngagementCampaignDto> CreateEngagementCampaignAsync(CreateEngagementCampaignRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _engagementCampaignRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<EngagementCampaignDto?> UpdateEngagementCampaignAsync(Guid id, UpdateEngagementCampaignRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _engagementCampaignRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _engagementCampaignRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteEngagementCampaignAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _engagementCampaignRepository.RemoveAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PulseSurveyDto>> GetPulseSurveysAsync(CancellationToken cancellationToken = default)
    {
        var surveys = await _pulseSurveyRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return surveys.Select(survey => survey.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<PulseSurveyDto?> GetPulseSurveyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var survey = await _pulseSurveyRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return survey?.ToDto();
    }

    /// <inheritdoc />
    public async Task<PulseSurveyDto> CreatePulseSurveyAsync(CreatePulseSurveyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _pulseSurveyRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<PulseSurveyDto?> UpdatePulseSurveyAsync(Guid id, UpdatePulseSurveyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _pulseSurveyRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _pulseSurveyRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeletePulseSurveyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _pulseSurveyRepository.RemoveAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<RecognitionProgramDto>> GetRecognitionProgramsAsync(CancellationToken cancellationToken = default)
    {
        var programs = await _recognitionProgramRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return programs.Select(program => program.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<RecognitionProgramDto?> GetRecognitionProgramByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var program = await _recognitionProgramRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return program?.ToDto();
    }

    /// <inheritdoc />
    public async Task<RecognitionProgramDto> CreateRecognitionProgramAsync(CreateRecognitionProgramRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var created = await _recognitionProgramRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<RecognitionProgramDto?> UpdateRecognitionProgramAsync(Guid id, UpdateRecognitionProgramRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _recognitionProgramRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existing);
        var persisted = await _recognitionProgramRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteRecognitionProgramAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _recognitionProgramRepository.RemoveAsync(id, cancellationToken);
    }
}
