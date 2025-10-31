using HR.Api.Filters;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using HR.Application.Mappings;
using HR.Application.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/leave")]
[Authorize(Roles = "Admin,HR,Manager,Employee")]
[AuditResource("Leave")]
[FeatureRequirement(HrFeature.LeaveManagement)]
public sealed class LeaveController(ILeaveService leaveService, ILeaveRequestRepository legacyLeaveRepo, ILeaveTypeRepository leaveTypeRepo) : ControllerBase
{
    private readonly ILeaveService _leaveService = leaveService;
    private readonly ILeaveRequestRepository _legacyLeaveRepo = legacyLeaveRepo;
    private readonly ILeaveTypeRepository _leaveTypeRepo = leaveTypeRepo;

    [HttpGet("types")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LeaveTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveTypes(CancellationToken cancellationToken)
    {
        var types = await _leaveService.GetLeaveTypesAsync(cancellationToken).ConfigureAwait(false);
        return Ok(types);
    }

    [HttpGet("balances")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LeaveBalanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalances([FromQuery] Guid employeeId, [FromQuery] int year, CancellationToken cancellationToken)
    {
        var balances = await _leaveService.GetBalancesAsync(employeeId, year, cancellationToken).ConfigureAwait(false);
        return Ok(balances);
    }

    public sealed class NewLeaveRequestInput : IValidatableRequest
    {
        public Guid EmployeeId { get; init; }
        public Guid LeaveTypeId { get; init; }
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
        public string? Reason { get; init; }
        public bool Draft { get; init; }
    }

    [HttpPost("requests")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRequest([FromBody] NewLeaveRequestInput request, CancellationToken cancellationToken)
    {
        if (request.Draft)
        {
            var leaveType = await _leaveTypeRepo.GetByIdAsync(request.LeaveTypeId, cancellationToken).ConfigureAwait(false);
            if (leaveType is null) return BadRequest("Leave type not found");

            var entity = new HR.Domain.Entities.LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                LeaveType = leaveType.Code,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason?.Trim() ?? string.Empty,
                Status = "Draft",
                RequestedAtUtc = DateTime.UtcNow
            };

            var created = await _legacyLeaveRepo.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            var dto = created.ToDto();
            return CreatedAtAction(nameof(GetRequestById), new { id = dto.Id }, dto);
        }
        else
        {
            var dto = await _leaveService.SubmitAsync(new SubmitLeaveRequest
            {
                EmployeeId = request.EmployeeId,
                LeaveTypeId = request.LeaveTypeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason
            }, cancellationToken).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetRequestById), new { id = dto.Id }, dto);
        }
    }

    [HttpGet("requests")] 
    [ProducesResponseType(typeof(PagedLeaveRequestsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRequests([FromQuery] Guid? employeeId, [FromQuery] Guid? managerId, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var pageResult = await _leaveService.GetRequestsAsync(employeeId, managerId, status, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Ok(pageResult);
    }

    [HttpGet("requests/{id:guid}")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRequestById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _legacyLeaveRepo.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return item is null ? NotFound() : Ok(item.ToDto());
    }

    [HttpPost("requests/{id:guid}:submit")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _legacyLeaveRepo.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null) return NotFound();
        if (!string.Equals(existing.Status, "Draft", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only draft requests can be submitted.");

        // Validate using preview to ensure sufficient balance and overlaps
        var types = await _leaveTypeRepo.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var leaveType = types.FirstOrDefault(t => t.Code.Equals(existing.LeaveType, StringComparison.OrdinalIgnoreCase));
        if (leaveType is null) return BadRequest("Invalid leave type.");

        // Overlap detection against pending/approved
        var all = await _legacyLeaveRepo.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var overlap = all.Any(r => r.EmployeeId == existing.EmployeeId &&
                                   (string.Equals(r.Status, "Pending", StringComparison.OrdinalIgnoreCase) || string.Equals(r.Status, "Approved", StringComparison.OrdinalIgnoreCase)) &&
                                   r.Id != existing.Id &&
                                   existing.StartDate <= r.EndDate && r.StartDate <= existing.EndDate);
        if (overlap)
            return BadRequest("Overlapping leave request exists.");

        var preview = await _leaveService.PreviewAsync(existing.EmployeeId, leaveType.Id, existing.StartDate, existing.EndDate, cancellationToken).ConfigureAwait(false);
        if (preview.AvailableAfter < 0)
            return BadRequest("Insufficient leave balance.");

        var updated = new HR.Domain.Entities.LeaveRequest
        {
            Id = existing.Id,
            EmployeeId = existing.EmployeeId,
            LeaveType = existing.LeaveType,
            StartDate = existing.StartDate,
            EndDate = existing.EndDate,
            Reason = existing.Reason,
            Status = "Pending",
            ApproverId = null,
            RequestedAtUtc = existing.RequestedAtUtc,
            DecisionAtUtc = null
        };

        var persisted = await _legacyLeaveRepo.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);
        return Ok(persisted!.ToDto());
    }

    [HttpPost("requests/{id:guid}:approve")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve(Guid id, [FromQuery] Guid managerId, CancellationToken cancellationToken)
        => Ok(await _leaveService.ApproveAsync(id, managerId, cancellationToken).ConfigureAwait(false));

    [HttpPost("requests/{id:guid}:reject")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject(Guid id, [FromQuery] Guid managerId, [FromBody] string reason, CancellationToken cancellationToken)
        => Ok(await _leaveService.RejectAsync(id, managerId, reason, cancellationToken).ConfigureAwait(false));

    [HttpPost("requests/{id:guid}:cancel")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] Guid employeeId, CancellationToken cancellationToken)
        => Ok(await _leaveService.CancelAsync(id, employeeId, cancellationToken).ConfigureAwait(false));
}
