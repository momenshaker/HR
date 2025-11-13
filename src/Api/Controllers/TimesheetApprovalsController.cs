using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using HR.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR,Manager")]
[AuditResource("TimesheetApproval")]
[FeatureRequirement(HrFeature.AttendanceAndTimeTracking)]
public sealed class TimesheetApprovalsController(ITimesheetService timesheetService) : ControllerBase
{
    private readonly ITimesheetService _service = timesheetService;

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<TimesheetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid managerId, [FromQuery] TimesheetStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var approvals = await _service.GetApprovalsAsync(managerId, status, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Ok(approvals);
    }
}


