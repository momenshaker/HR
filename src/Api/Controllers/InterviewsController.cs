using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides endpoints for interview scheduling and management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[FeatureRequirement(HrFeature.RecruitmentAndAts)]
public sealed class InterviewsController(IRecruitmentService recruitmentService) : ControllerBase
{
    private readonly IRecruitmentService _recruitmentService = recruitmentService;

    /// <summary>
    ///     Retrieves interview schedules with optional filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<InterviewScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] Guid? vacancyId,
        [FromQuery] Guid? candidateId,
        [FromQuery] bool onlyUpcoming,
        CancellationToken cancellationToken)
    {
        var interviews = await _recruitmentService
            .GetInterviewsAsync(vacancyId, candidateId, onlyUpcoming, cancellationToken)
            .ConfigureAwait(false);
        return Ok(interviews);
    }

    /// <summary>
    ///     Schedules a new interview.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InterviewScheduleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] ScheduleInterviewRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var interview = await _recruitmentService.ScheduleInterviewAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { interview.Id }, interview);
    }

    /// <summary>
    ///     Updates an existing interview schedule.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(InterviewScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateInterviewScheduleRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _recruitmentService.UpdateInterviewAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Cancels a scheduled interview.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var cancelled = await _recruitmentService.CancelInterviewAsync(id, cancellationToken).ConfigureAwait(false);
        return cancelled ? NoContent() : NotFound();
    }
}
