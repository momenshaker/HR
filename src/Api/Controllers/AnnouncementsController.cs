using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Infrastructure.Options;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for internal communications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[FeatureRequirement(HrFeature.InternalCommunication)]
public sealed class AnnouncementsController : ControllerBase
{
    private readonly ICommunicationService _communicationService;

    public AnnouncementsController(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    /// <summary>
    ///     Retrieves all announcements.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AnnouncementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var announcements = await _communicationService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(announcements);
    }

    /// <summary>
    ///     Retrieves an announcement by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AnnouncementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var announcement = await _communicationService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return announcement is null ? NotFound() : Ok(announcement);
    }

    /// <summary>
    ///     Creates a new announcement.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AnnouncementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateAnnouncementRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdAnnouncement = await _communicationService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdAnnouncement.Id }, createdAnnouncement);
    }

    /// <summary>
    ///     Updates an existing announcement.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AnnouncementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateAnnouncementRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedAnnouncement = await _communicationService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedAnnouncement is null ? NotFound() : Ok(updatedAnnouncement);
    }

    /// <summary>
    ///     Deletes an announcement.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _communicationService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
