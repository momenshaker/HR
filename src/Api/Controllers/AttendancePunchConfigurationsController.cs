using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin,HR,Manager")]
[FeatureRequirement(HrFeature.AttendanceAndTimeTracking)]
public sealed class AttendancePunchConfigurationsController(
    IAttendancePunchConfigurationService configurationService) : ControllerBase
{
    private readonly IAttendancePunchConfigurationService _configurationService = configurationService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AttendancePunchConfigurationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var configuration = await _configurationService
            .GetPunchTypesAsync(cancellationToken)
            .ConfigureAwait(false);

        return Ok(configuration);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AttendancePunchConfigurationDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync(
        [FromBody] AttendancePunchConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        var created = await _configurationService.SaveAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AttendancePunchConfigurationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutAsync(
        Guid id,
        [FromBody] AttendancePunchConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Id is null || request.Id != id)
        {
            request = request with { Id = id };
        }

        var updated = await _configurationService.SaveAsync(request, cancellationToken).ConfigureAwait(false);
        return Ok(updated);
    }
}
