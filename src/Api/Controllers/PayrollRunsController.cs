using HR.Api.Filters;
using HR.Api.Middleware;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for payroll management operations.
/// </summary>
[ApiController]
[Route("api/payroll/runs")]
[Authorize(Roles = "Admin,HR")]
[AuditResource("PayrollRun")]
[FeatureRequirement(HrFeature.PayrollManagement)]
[RequiresSubscriptionEntitlement(HrFeature.PayrollManagement)]
public sealed class PayrollRunsController(IPayrollService payrollService) : ControllerBase
{
    private readonly IPayrollService _payrollService = payrollService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PayrollRunDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery] Guid? orgId = null, [FromQuery] string? status = null, CancellationToken cancellationToken = default)
    {
        var runs = await _payrollService.GetRuns(orgId, status, cancellationToken).ConfigureAwait(false);
        return Ok(runs);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PayrollRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var run = await _payrollService.GetRun(id, cancellationToken).ConfigureAwait(false);
        return run is null ? NotFound() : Ok(run);
    }

    [HttpGet("{id:guid}/items")]
    [ProducesResponseType(typeof(IReadOnlyCollection<PayrollItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var items = await _payrollService.GetItems(id, cancellationToken).ConfigureAwait(false);
        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PayrollRunDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PostAsync([FromBody] CreatePayrollRunRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var createdRun = await _payrollService.CreateRun(request.OrganizationId, request.PeriodStart, request.PeriodEnd, cancellationToken).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdRun.Id }, createdRun);
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}:calculate")]
    [ProducesResponseType(typeof(PayrollRunDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CalculateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var run = await _payrollService.Calculate(id, cancellationToken).ConfigureAwait(false);
        return Ok(run);
    }

    [HttpPost("{id:guid}:approve")]
    [ProducesResponseType(typeof(PayrollRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var run = await _payrollService.Approve(id, cancellationToken).ConfigureAwait(false);
            return Ok(run);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}:paid")]
    [ProducesResponseType(typeof(PayrollRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkPaidAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var run = await _payrollService.MarkPaid(id, cancellationToken).ConfigureAwait(false);
            return Ok(run);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}:payslips")] // Optional trigger endpoint
    [ProducesResponseType(typeof(IReadOnlyCollection<PayslipDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GeneratePayslipsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var slips = await _payrollService.GeneratePayslips(id, cancellationToken).ConfigureAwait(false);
        return Ok(slips);
    }

    [HttpGet("~/api/payroll/payslips")]
    [ProducesResponseType(typeof(IReadOnlyCollection<PayslipDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayslipsAsync(
        [FromQuery] Guid employeeId,
        [FromQuery] DateOnly? periodStart = null,
        [FromQuery] DateOnly? periodEnd = null,
        CancellationToken cancellationToken = default)
    {
        var slips = await _payrollService.GetPayslips(employeeId, periodStart, periodEnd, cancellationToken).ConfigureAwait(false);
        return Ok(slips);
    }
}
