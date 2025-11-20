using System.Threading;
using HR.Api.Contracts;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Orchestrates the anonymous onboarding journey for new tenants.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/onboarding")]
[AllowAnonymous]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class OnboardingController(IOnboardingService onboardingService) : ControllerBase
{
    private readonly IOnboardingService _onboardingService = onboardingService;

    [HttpPost]
    [ProducesResponseType(typeof(OnboardingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PostAsync([FromBody] OnboardingRequest request, CancellationToken cancellationToken)
    {
        var result = await _onboardingService.StartAsync(request, cancellationToken).ConfigureAwait(false);
        var response = new OnboardingResponse(
            result.CustomerId,
            result.OrganizationId,
            result.SubscriptionId,
            result.AdminUserId,
            result.AdminEmployeeId);
        return Created(string.Empty, response);
    }
}
