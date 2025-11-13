using System;
using System.Reflection;
using HR.Api.Filters;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides system metadata and operational health endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[AllowAnonymous]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class SystemController(IHostEnvironment hostEnvironment) : ControllerBase
{
    private static readonly string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    /// <summary>
    ///     Gets the runtime health status of the API.
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(SystemHealthResponse), StatusCodes.Status200OK)]
    public ActionResult<SystemHealthResponse> GetHealth()
    {
        var response = new SystemHealthResponse("Healthy", _hostEnvironment.EnvironmentName, DateTimeOffset.UtcNow);
        return Ok(response);
    }

    /// <summary>
    ///     Returns the current application version and environment metadata.
    /// </summary>
    [HttpGet("version")]
    [ProducesResponseType(typeof(SystemVersionResponse), StatusCodes.Status200OK)]
    public ActionResult<SystemVersionResponse> GetVersion()
    {
        var response = new SystemVersionResponse(AssemblyVersion, _hostEnvironment.EnvironmentName);
        return Ok(response);
    }

    public sealed record SystemHealthResponse(string Status, string Environment, DateTimeOffset Timestamp);

    public sealed record SystemVersionResponse(string Version, string Environment);
}

