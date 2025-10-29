using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for employee recognition programmes.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR,Manager")]
[AuditResource("RecognitionProgram")]
[FeatureRequirement(HrFeature.InternalCommunication)]
public sealed class RecognitionProgramsController(ICommunicationService communicationService) : ControllerBase
{
    private readonly ICommunicationService _communicationService = communicationService;

    /// <summary>
    ///     Retrieves all recognition programmes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<RecognitionProgramDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var programs = await _communicationService.GetRecognitionProgramsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(programs);
    }

    /// <summary>
    ///     Retrieves a recognition programme by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecognitionProgramDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var program = await _communicationService.GetRecognitionProgramByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return program is null ? NotFound() : Ok(program);
    }

    /// <summary>
    ///     Creates a new recognition programme.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecognitionProgramDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateRecognitionProgramRequest request, CancellationToken cancellationToken)
    {

        var createdProgram = await _communicationService
            .CreateRecognitionProgramAsync(request, cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdProgram.Id }, createdProgram);
    }

    /// <summary>
    ///     Updates an existing recognition programme.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RecognitionProgramDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateRecognitionProgramRequest request, CancellationToken cancellationToken)
    {

        var updatedProgram = await _communicationService
            .UpdateRecognitionProgramAsync(id, request, cancellationToken)
            .ConfigureAwait(false);

        return updatedProgram is null ? NotFound() : Ok(updatedProgram);
    }

    /// <summary>
    ///     Deletes a recognition programme.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _communicationService
            .DeleteRecognitionProgramAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return deleted ? NoContent() : NotFound();
    }
}
