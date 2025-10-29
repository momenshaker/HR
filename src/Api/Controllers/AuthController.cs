using HR.Api.Contracts;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides authentication endpoints for obtaining JWT bearer tokens.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[AllowAnonymous]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    /// <summary>
    ///     Authenticates the supplied credentials and returns an access token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService
            .AuthenticateAsync(request.Email, request.Password, cancellationToken)
            .ConfigureAwait(false);

        if (result is null)
        {
            var error = new ErrorResponse("invalid_credentials", "Invalid email or password.", HttpContext.TraceIdentifier);
            return BadRequest(error);
        }

        var response = new AuthResponse(
            result.AccessToken,
            result.TokenType,
            (int)result.ExpiresIn.TotalSeconds,
            result.RefreshToken);

        return Ok(response);
    }
}
