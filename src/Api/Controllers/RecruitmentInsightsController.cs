using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides collaborative hiring analytics endpoints.
/// </summary>
[ApiController]
[Route("api/recruitment/insights")]
[FeatureRequirement(HrFeature.RecruitmentAndAts)]
public sealed class RecruitmentInsightsController(IRecruitmentService recruitmentService) : ControllerBase
{
    private readonly IRecruitmentService _recruitmentService = recruitmentService;

    /// <summary>
    ///     Retrieves aggregated recruitment insights for hiring teams.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(RecruitmentInsightsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var insights = await _recruitmentService.GetInsightsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(insights);
    }
}
