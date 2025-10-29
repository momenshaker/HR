using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using HR.Api.Contracts;
using HR.Api.Filters;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using IAuthenticationService = HR.Application.Abstractions.Services.IAuthenticationService;

namespace HR.Api.Controllers;

/// <summary>
///     Provides authentication and identity management endpoints backed by ASP.NET Core Identity.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    /// <summary>
    ///     Authenticates the supplied credentials and returns an access token.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
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

    /// <summary>
    ///     Registers a new identity user account.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var claims = request.Claims?.ToDictionary(pair => pair.Key, pair => pair.Value)
                     ?? new Dictionary<string, string>();

        var (result, userId) = await _authenticationService.RegisterUserAsync(
            request.Email,
            request.Password,
            request.CustomerId,
            request.Roles ?? Array.Empty<string>(),
            claims,
            cancellationToken).ConfigureAwait(false);

        if (!result.Succeeded || userId is null)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "registration_failed", "Failed to register the account."));
        }

        var confirmationToken = await _authenticationService
            .GenerateEmailConfirmationTokenAsync(userId.Value, cancellationToken)
            .ConfigureAwait(false);

        var response = new RegistrationResponse(userId.Value, confirmationToken);
        return Ok(response);
    }

    /// <summary>
    ///     Confirms a user's email address using the supplied token.
    /// </summary>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService
            .ConfirmEmailAsync(request.UserId, request.Token, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "email_confirmation_failed", "Email confirmation failed."));
        }

        return NoContent();
    }

    /// <summary>
    ///     Generates a new email confirmation token for the specified user.
    /// </summary>
    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResendConfirmationAsync([FromBody] ResendConfirmationRequest request, CancellationToken cancellationToken)
    {
        var token = await _authenticationService
            .GenerateEmailConfirmationTokenAsync(request.UserId, cancellationToken)
            .ConfigureAwait(false);

        if (token is null)
        {
            var error = new ErrorResponse("user_not_found", "User was not found.", HttpContext.TraceIdentifier);
            return NotFound(error);
        }

        return Ok(new TokenResponse(request.UserId, token, "email_confirmation"));
    }

    /// <summary>
    ///     Generates a password reset token for the specified user.
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = await _authenticationService
            .GetUserIdByEmailAsync(request.Email, cancellationToken)
            .ConfigureAwait(false);

        if (!userId.HasValue)
        {
            var error = new ErrorResponse("user_not_found", "User was not found.", HttpContext.TraceIdentifier);
            return NotFound(error);
        }

        var token = await _authenticationService
            .GeneratePasswordResetTokenAsync(userId.Value, cancellationToken)
            .ConfigureAwait(false);

        if (token is null)
        {
            var error = new ErrorResponse("token_generation_failed", "Unable to generate password reset token.", HttpContext.TraceIdentifier);
            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }

        return Ok(new TokenResponse(userId.Value, token, "password_reset"));
    }

    /// <summary>
    ///     Resets a user's password using a valid verification token.
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService
            .ResetPasswordAsync(request.UserId, request.Token, request.NewPassword, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "password_reset_failed", "Password reset failed."));
        }

        return NoContent();
    }

    /// <summary>
    ///     Changes the password for the currently authenticated user.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result = await _authenticationService
            .ChangePasswordAsync(userId.Value, request.CurrentPassword, request.NewPassword, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "password_change_failed", "Password change failed."));
        }

        return NoContent();
    }

    /// <summary>
    ///     Retrieves the roles assigned to the specified user.
    /// </summary>
    [HttpGet("users/{userId:guid}/roles")]
    [Authorize]
    [ProducesResponseType(typeof(UserRolesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roles = await _authenticationService
            .GetRolesAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(new UserRolesResponse(userId, roles));
    }

    /// <summary>
    ///     Adds the provided roles to the specified user.
    /// </summary>
    [HttpPost("users/{userId:guid}/roles")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRolesAsync(Guid userId, [FromBody] UpdateUserRolesRequest request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService
            .AddToRolesAsync(userId, request.Roles, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "role_assignment_failed", "Unable to assign roles."));
        }

        return NoContent();
    }

    /// <summary>
    ///     Removes the provided roles from the specified user.
    /// </summary>
    [HttpDelete("users/{userId:guid}/roles")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveRolesAsync(Guid userId, [FromBody] UpdateUserRolesRequest request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService
            .RemoveFromRolesAsync(userId, request.Roles, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "role_removal_failed", "Unable to remove roles."));
        }

        return NoContent();
    }

    /// <summary>
    ///     Retrieves the claims assigned to the specified user.
    /// </summary>
    [HttpGet("users/{userId:guid}/claims")]
    [Authorize]
    [ProducesResponseType(typeof(UserClaimsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClaimsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var claims = await _authenticationService
            .GetClaimsAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        var response = new UserClaimsResponse(
            userId,
            claims.ToDictionary(claim => claim.Type, claim => claim.Value, StringComparer.OrdinalIgnoreCase));

        return Ok(response);
    }

    /// <summary>
    ///     Adds the supplied claims to the specified user.
    /// </summary>
    [HttpPost("users/{userId:guid}/claims")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddClaimsAsync(Guid userId, [FromBody] UpdateUserClaimsRequest request, CancellationToken cancellationToken)
    {
        var claims = request.Claims?.ToDictionary(pair => pair.Key, pair => pair.Value)
                     ?? new Dictionary<string, string>();

        var result = await _authenticationService
            .AddClaimsAsync(userId, claims, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "claim_assignment_failed", "Unable to assign claims."));
        }

        return NoContent();
    }

    /// <summary>
    ///     Removes the supplied claims from the specified user.
    /// </summary>
    [HttpDelete("users/{userId:guid}/claims")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveClaimsAsync(Guid userId, [FromBody] UpdateUserClaimsRequest request, CancellationToken cancellationToken)
    {
        var claims = request.Claims?.ToDictionary(pair => pair.Key, pair => pair.Value)
                     ?? new Dictionary<string, string>();

        var result = await _authenticationService
            .RemoveClaimsAsync(userId, claims, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "claim_removal_failed", "Unable to remove claims."));
        }

        return NoContent();
    }

    /// <summary>
    ///     Updates the lockout configuration for the specified user.
    /// </summary>
    [HttpPost("users/{userId:guid}/lockout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetLockoutAsync(Guid userId, [FromBody] LockoutRequest request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService
            .SetLockoutAsync(userId, request.Enabled, request.LockoutEnd, cancellationToken)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "lockout_update_failed", "Unable to update lockout configuration."));
        }

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var identifier = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(identifier, out var parsed) ? parsed : null;
    }

    private ErrorResponse CreateIdentityErrorResponse(IdentityResult result, string errorCode, string message)
    {
        var details = result.Errors
            .Select(error => new ErrorDetail(error.Code, error.Description))
            .ToArray();

        return new ErrorResponse(errorCode, message, HttpContext.TraceIdentifier)
        {
            Details = details
        };
    }
}
