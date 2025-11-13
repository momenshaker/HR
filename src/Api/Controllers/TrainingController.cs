using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Lightweight Training API.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public sealed class TrainingController(ILightweightTrainingService trainingService) : ControllerBase
{
    private readonly ILightweightTrainingService _service = trainingService;

    [HttpGet("courses")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LiteCourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoursesAsync([FromQuery] Guid orgId, CancellationToken cancellationToken)
    {
        var courses = await _service.GetCoursesAsync(orgId, cancellationToken).ConfigureAwait(false);
        return Ok(courses);
    }

    [HttpPost("courses")]
    [ProducesResponseType(typeof(LiteCourseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCourseAsync([FromBody] CreateLiteCourseRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateCourseAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetCourseSessionsAsync), new { id = created.Id }, created);
    }

    [HttpGet("courses/{id:guid}/sessions")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LiteCourseSessionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseSessionsAsync(Guid id, CancellationToken cancellationToken)
    {
        var sessions = await _service.GetCourseSessionsAsync(id, cancellationToken).ConfigureAwait(false);
        return Ok(sessions);
    }

    [HttpPost("sessions")]
    [ProducesResponseType(typeof(LiteCourseSessionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSessionAsync([FromBody] CreateLiteCourseSessionRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateCourseSessionAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetCourseSessionsAsync), new { id = created.CourseId }, created);
    }

    [HttpPost("sessions/{sessionId:guid}/enroll")]
    [ProducesResponseType(typeof(LiteEnrollmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> EnrollAsync(Guid sessionId, [FromQuery] Guid employeeId, CancellationToken cancellationToken)
    {
        var dto = await _service.EnrollAsync(sessionId, employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(dto);
    }

    [HttpPost("sessions/{sessionId:guid}/complete")]
    [ProducesResponseType(typeof(LiteEnrollmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CompleteAsync(Guid sessionId, [FromQuery] Guid employeeId, CancellationToken cancellationToken)
    {
        var dto = await _service.CompleteAsync(sessionId, employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(dto);
    }

    [HttpPost("sessions/{sessionId:guid}/cancel")]
    [ProducesResponseType(typeof(LiteEnrollmentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelAsync(Guid sessionId, [FromQuery] Guid employeeId, CancellationToken cancellationToken)
    {
        var dto = await _service.CancelAsync(sessionId, employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(dto);
    }
}


