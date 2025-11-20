using HR.Api.Contracts;
using HR.Api.Filters;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Infrastructure.Persistence.EntityFramework;
using HR.Infrastructure.Security.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IAuthenticationService = HR.Application.Abstractions.Services.IAuthenticationService;

namespace HR.Api.Controllers;

/// <summary>
///     Provides authentication and identity management endpoints backed by ASP.NET Core Identity.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly HrDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IEmployeeRepository _employeeRepository;

    public AuthController(
        IAuthenticationService authenticationService,
        IJwtTokenService jwtTokenService,
        HrDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IEmployeeRepository employeeRepository)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
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
    ///     Registers a new user and links to an existing employee.
    /// </summary>
    [HttpPost("register-employee")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegisterEmployeeAsync([FromBody] RegisterEmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken).ConfigureAwait(false);
        if (employee is null)
        {
            return UnprocessableEntity(new ErrorResponse("invalid_employee", "Employee does not exist.", HttpContext.TraceIdentifier));
        }

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Conflict(new ErrorResponse("duplicate_email", "Email already in use.", HttpContext.TraceIdentifier));
        }
        if (await _userManager.FindByNameAsync(request.UserName) is not null)
        {
            return Conflict(new ErrorResponse("duplicate_username", "Username already in use.", HttpContext.TraceIdentifier));
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim(),
            UserName = request.UserName.Trim(),
            EmailConfirmed = true,
            EmployeeId = request.EmployeeId
        };

        var result = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "registration_failed", "Failed to register the account."));
        }

        // Ensure Employee role
        const string employeeRole = "Employee";
        if (!await _userManager.IsInRoleAsync(user, employeeRole).ConfigureAwait(false))
        {
            await _userManager.AddToRoleAsync(user, employeeRole).ConfigureAwait(false);
        }

        var response = new RegistrationResponse(user.Id, null);
        return CreatedAtAction(nameof(GetMeAsync), new { userId = user.Id }, response);
    }

    /// <summary>
    ///     Exchanges a refresh token for a new access token and refresh token (rotation).
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var token = await _dbContext.UserRefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken).ConfigureAwait(false);
        if (token is null || token.RevokedAtUtc is not null || token.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return Unauthorized(new ErrorResponse("invalid_refresh_token", "Refresh token is invalid or expired.", HttpContext.TraceIdentifier));
        }

        var user = await _userManager.FindByIdAsync(token.UserId.ToString()).ConfigureAwait(false);
        if (user is null)
        {
            return Unauthorized(new ErrorResponse("invalid_user", "User not found.", HttpContext.TraceIdentifier));
        }

        // Rotate: revoke old token
        token.RevokedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var jwtOptions = HttpContext.RequestServices.GetRequiredService<IOptions<HR.Infrastructure.Options.JwtOptions>>().Value;
        var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        var additionalClaims = new List<Claim>(claims)
        {
            new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };
        foreach (var role in roles)
        {
            additionalClaims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, role));
        }
        if (user.EmployeeId.HasValue)
        {
            var employeeClaimName = jwtOptions.EmployeeIdClaim;
            additionalClaims.Add(new System.Security.Claims.Claim(employeeClaimName, user.EmployeeId.Value.ToString()));
        }

        var accessToken = _jwtTokenService.CreateAccessToken(additionalClaims);
        var (newRefresh, refreshExpires) = _jwtTokenService.CreateRefreshToken();
        var newHash = HashToken(newRefresh);
        _dbContext.UserRefreshTokens.Add(new UserRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newHash,
            ExpiresAtUtc = refreshExpires
        });
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var expiresIn = TimeSpan.FromMinutes(jwtOptions.AccessTokenMinutes);
        return Ok(new AuthResponse(accessToken, "Bearer", (int)expiresIn.TotalSeconds, newRefresh));
    }

    /// <summary>
    ///     Returns information about the current user.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMeAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId.Value.ToString()).ConfigureAwait(false);
        if (user is null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        var response = new MeResponse(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty, user.EmployeeId, roles.ToArray());
        return Ok(response);
    }

    /// <summary>
    ///     Links an existing user to an employee. Admin only.
    /// </summary>
    [HttpPost("link-employee")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> LinkEmployeeAsync([FromBody] RegisterEmployeeRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UserName).ConfigureAwait(false)
                   ?? await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
        if (user is null)
        {
            return UnprocessableEntity(new ErrorResponse("user_not_found", "User not found.", HttpContext.TraceIdentifier));
        }

        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken).ConfigureAwait(false);
        if (employee is null)
        {
            return UnprocessableEntity(new ErrorResponse("invalid_employee", "Employee does not exist.", HttpContext.TraceIdentifier));
        }

        user.EmployeeId = request.EmployeeId;
        var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return BadRequest(CreateIdentityErrorResponse(result, "link_failed", "Unable to link employee."));
        }

        return NoContent();
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
            employeeId: null,
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

    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private ErrorResponse CreateIdentityErrorResponse(IdentityResult result, string errorCode, string message)
    {
        var details = result.Errors
            .Select(error => new ErrorDetail(string.Empty, error.Description, error.Code))
            .ToArray();

        return new ErrorResponse(errorCode, message, HttpContext.TraceIdentifier)
        {
            Details = details
        };
    }
}

