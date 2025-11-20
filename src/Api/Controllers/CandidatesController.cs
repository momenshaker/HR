using HR.Api.Authorization;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for recruitment and applicant tracking operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[RolePermission(
    "recruitment",
    readRoles: new[] { "Admin", "HR", "Manager" },
    writeRoles: new[] { "Admin", "HR", "Manager" })]
[AuditResource("Candidate")]
[FeatureRequirement(HrFeature.RecruitmentAndAts)]
public sealed class CandidatesController(IRecruitmentService recruitmentService) : ControllerBase
{
    private readonly IRecruitmentService _recruitmentService = recruitmentService;

    /// <summary>
    ///     Retrieves all candidates in the applicant tracking pipeline.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CandidateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var candidates = await _recruitmentService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(candidates);
    }

    /// <summary>
    ///     Retrieves a candidate by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CandidateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var candidate = await _recruitmentService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return candidate is null ? NotFound() : Ok(candidate);
    }

    /// <summary>
    ///     Creates a new candidate record.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CandidateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateCandidateRequest request, CancellationToken cancellationToken)
    {

        var createdCandidate = await _recruitmentService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdCandidate.Id }, createdCandidate);
    }

    /// <summary>
    ///     Updates an existing candidate record.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CandidateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateCandidateRequest request, CancellationToken cancellationToken)
    {

        var updatedCandidate = await _recruitmentService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedCandidate is null ? NotFound() : Ok(updatedCandidate);
    }

    /// <summary>
    ///     Advances a candidate to the next pipeline stage and optionally schedules interviews.
    /// </summary>
    [HttpPost("{id:guid}/advance")]
    [ProducesResponseType(typeof(CandidateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdvanceAsync(Guid id, [FromBody] AdvanceCandidateRequest request, CancellationToken cancellationToken)
    {

        var advancedCandidate = await _recruitmentService.AdvanceCandidateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return advancedCandidate is null ? NotFound() : Ok(advancedCandidate);
    }

    /// <summary>
    ///     Deletes a candidate record.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _recruitmentService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
