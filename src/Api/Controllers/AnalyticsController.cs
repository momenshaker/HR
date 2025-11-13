using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR")]
[AuditResource("Analytics")]
[FeatureRequirement(HrFeature.HrAnalytics)]
public sealed class AnalyticsController(IAnalyticsQueryService analytics) : ControllerBase
{
    private readonly IAnalyticsQueryService _analytics = analytics;

    [HttpGet("headcount")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HeadcountItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHeadcountAsync([FromQuery] Guid orgId, [FromQuery] Guid? departmentId, CancellationToken cancellationToken)
    {
        var result = await _analytics.GetHeadcountAsync(orgId, departmentId, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("utilization")]
    [ProducesResponseType(typeof(IReadOnlyCollection<UtilizationPeriodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUtilizationAsync([FromQuery] Guid orgId, [FromQuery(Name = "from")] DateOnly start, [FromQuery(Name = "to")] DateOnly end, CancellationToken cancellationToken)
    {
        var result = await _analytics.GetUtilizationAsync(orgId, start, end, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("leave-usage")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LeaveUsageItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveUsageAsync([FromQuery] Guid orgId, [FromQuery] int year, CancellationToken cancellationToken)
    {
        var result = await _analytics.GetLeaveUsageAsync(orgId, year, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("payroll-totals")]
    [ProducesResponseType(typeof(PayrollTotalsResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayrollTotalsAsync([FromQuery] Guid orgId, [FromQuery(Name = "from")] DateOnly start, [FromQuery(Name = "to")] DateOnly end, CancellationToken cancellationToken)
    {
        var result = await _analytics.GetPayrollTotalsAsync(orgId, start, end, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("recruitment-funnel")]
    [ProducesResponseType(typeof(IReadOnlyCollection<StageCountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecruitmentFunnelAsync([FromQuery] Guid jobId, CancellationToken cancellationToken)
    {
        var result = await _analytics.GetRecruitmentFunnelAsync(jobId, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("training-compliance")]
    [ProducesResponseType(typeof(TrainingComplianceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrainingComplianceAsync([FromQuery] Guid orgId, CancellationToken cancellationToken)
    {
        var result = await _analytics.GetTrainingComplianceAsync(orgId, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }
}

