using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Exposes platform-wide configuration, including feature availability, for client discovery.
/// </summary>
[ApiController]
[Route("api/platform/configuration")]
public sealed class PlatformConfigurationController(IPlatformConfigurationService configurationService) : ControllerBase
{
    private readonly IPlatformConfigurationService _configurationService = configurationService;

    /// <summary>
    ///     Retrieves the current platform configuration and feature catalogue.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PlatformConfigurationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var configuration = await _configurationService.GetConfigurationAsync(cancellationToken).ConfigureAwait(false);
        return Ok(configuration);
    }
}
