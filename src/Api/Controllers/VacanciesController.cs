using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides endpoints for publishing and managing vacancies.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR")]
[AuditResource("Vacancy")]
[FeatureRequirement(HrFeature.RecruitmentAndAts)]
public sealed class VacanciesController(IRecruitmentService recruitmentService) : ControllerBase
{
    private readonly IRecruitmentService _recruitmentService = recruitmentService;

    /// <summary>
    ///     Retrieves all published vacancies.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<VacancyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var vacancies = await _recruitmentService.GetVacanciesAsync(cancellationToken).ConfigureAwait(false);
        return Ok(vacancies);
    }

    /// <summary>
    ///     Retrieves a vacancy by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var vacancy = await _recruitmentService.GetVacancyByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return vacancy is null ? NotFound() : Ok(vacancy);
    }

    /// <summary>
    ///     Publishes a new vacancy.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateVacancyRequest request, CancellationToken cancellationToken)
    {

        var vacancy = await _recruitmentService.PublishVacancyAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = vacancy.Id }, vacancy);
    }

    /// <summary>
    ///     Updates an existing vacancy.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VacancyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateVacancyRequest request, CancellationToken cancellationToken)
    {

        var updated = await _recruitmentService.UpdateVacancyAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Closes a vacancy to stop receiving applications.
    /// </summary>
    [HttpPost("{id:guid}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseAsync(Guid id, CancellationToken cancellationToken)
    {
        var closed = await _recruitmentService.CloseVacancyAsync(id, cancellationToken).ConfigureAwait(false);
        return closed ? NoContent() : NotFound();
    }
}
