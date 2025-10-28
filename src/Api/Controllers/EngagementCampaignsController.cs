using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for orchestrating engagement campaigns.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[FeatureRequirement(HrFeature.InternalCommunication)]
public sealed class EngagementCampaignsController(ICommunicationService communicationService) : ControllerBase
{
    private readonly ICommunicationService _communicationService = communicationService;

    /// <summary>
    ///     Retrieves all engagement campaigns.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<EngagementCampaignDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var campaigns = await _communicationService.GetEngagementCampaignsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(campaigns);
    }

    /// <summary>
    ///     Retrieves an engagement campaign by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EngagementCampaignDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var campaign = await _communicationService.GetEngagementCampaignByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return campaign is null ? NotFound() : Ok(campaign);
    }

    /// <summary>
    ///     Creates a new engagement campaign.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EngagementCampaignDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateEngagementCampaignRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdCampaign = await _communicationService
            .CreateEngagementCampaignAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdCampaign.Id }, createdCampaign);
    }

    /// <summary>
    ///     Updates an existing engagement campaign.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EngagementCampaignDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateEngagementCampaignRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedCampaign = await _communicationService
            .UpdateEngagementCampaignAsync(id, request, cancellationToken)
            .ConfigureAwait(false);

        return updatedCampaign is null ? NotFound() : Ok(updatedCampaign);
    }

    /// <summary>
    ///     Deletes an engagement campaign.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _communicationService
            .DeleteEngagementCampaignAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return deleted ? NoContent() : NotFound();
    }
}
