using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for payroll management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class PayrollRunsController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollRunsController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    /// <summary>
    ///     Retrieves all payroll runs.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PayrollRunDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var runs = await _payrollService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(runs);
    }

    /// <summary>
    ///     Retrieves a payroll run by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PayrollRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var run = await _payrollService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return run is null ? NotFound() : Ok(run);
    }

    /// <summary>
    ///     Creates a new payroll run.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PayrollRunDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreatePayrollRunRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdRun = await _payrollService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdRun.Id }, createdRun);
    }

    /// <summary>
    ///     Updates an existing payroll run.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PayrollRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdatePayrollRunRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedRun = await _payrollService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedRun is null ? NotFound() : Ok(updatedRun);
    }

    /// <summary>
    ///     Deletes a payroll run.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _payrollService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
