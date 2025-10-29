using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for running employee pulse surveys.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR,Manager")]
[AuditResource("PulseSurvey")]
[FeatureRequirement(HrFeature.InternalCommunication)]
public sealed class PulseSurveysController(ICommunicationService communicationService) : ControllerBase
{
    private readonly ICommunicationService _communicationService = communicationService;

    /// <summary>
    ///     Retrieves all pulse surveys.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PulseSurveyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var surveys = await _communicationService.GetPulseSurveysAsync(cancellationToken).ConfigureAwait(false);
        return Ok(surveys);
    }

    /// <summary>
    ///     Retrieves a pulse survey by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PulseSurveyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var survey = await _communicationService.GetPulseSurveyByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return survey is null ? NotFound() : Ok(survey);
    }

    /// <summary>
    ///     Creates a new pulse survey.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PulseSurveyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreatePulseSurveyRequest request, CancellationToken cancellationToken)
    {

        var createdSurvey = await _communicationService
            .CreatePulseSurveyAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdSurvey.Id }, createdSurvey);
    }

    /// <summary>
    ///     Updates an existing pulse survey.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PulseSurveyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdatePulseSurveyRequest request, CancellationToken cancellationToken)
    {

        var updatedSurvey = await _communicationService
            .UpdatePulseSurveyAsync(id, request, cancellationToken)
            .ConfigureAwait(false);

        return updatedSurvey is null ? NotFound() : Ok(updatedSurvey);
    }

    /// <summary>
    ///     Deletes a pulse survey.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _communicationService
            .DeletePulseSurveyAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return deleted ? NoContent() : NotFound();
    }
}
