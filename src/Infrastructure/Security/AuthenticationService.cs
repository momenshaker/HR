using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Infrastructure.Options;
using HR.Infrastructure.Security.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace HR.Infrastructure.Security;

/// <summary>
///     Identity-backed authentication service that issues JWT bearer tokens and exposes common identity operations.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{

    private readonly IOptionsMonitor<AuthenticationOptions> _authenticationOptions;
    private readonly IOptionsMonitor<JwtOptions> _jwtOptions;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TimeProvider _timeProvider;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptionsMonitor<AuthenticationOptions> authenticationOptions,
        IOptionsMonitor<JwtOptions> jwtOptions,
        TimeProvider timeProvider)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _authenticationOptions = authenticationOptions ?? throw new ArgumentNullException(nameof(authenticationOptions));
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    /// <inheritdoc />
    public async Task<AuthenticationResult?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = await _userManager.FindByEmailAsync(email.Trim()).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true)
            .ConfigureAwait(false);

        if (!signInResult.Succeeded)
        {
            return null;
        }

        var jwtOptions = _jwtOptions.CurrentValue;
        if (string.IsNullOrWhiteSpace(jwtOptions.Key))
        {
            throw new InvalidOperationException("JWT signing key is not configured.");
        }

        var now = _timeProvider.GetUtcNow();
        var expires = now.AddMinutes(_authenticationOptions.CurrentValue.TokenLifetimeMinutes);

        var claims = await BuildClaimsAsync(user, jwtOptions.CustomerClaim, cancellationToken).ConfigureAwait(false);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            Subject = new ClaimsIdentity(claims),
            NotBefore = now.UtcDateTime,
            Expires = expires.UtcDateTime,
            SigningCredentials = signingCredentials
        };

        var securityToken = _tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = _tokenHandler.WriteToken(securityToken);

        return new AuthenticationResult(accessToken, expires - now, "Bearer", null);
    }

    /// <inheritdoc />
    public async Task<(IdentityResult Result, Guid? UserId)> RegisterUserAsync(
        string email,
        string password,
        string? customerId = null,
        IEnumerable<string>? roles = null,
        IDictionary<string, string>? claims = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(email))
        {
            return (IdentityResult.Failed(new IdentityError { Code = "invalid_email", Description = "Email is required." }), null);
        }

        var normalizedEmail = email.Trim();
        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail).ConfigureAwait(false);
        if (existingUser is not null)
        {
            return (IdentityResult.Failed(new IdentityError { Code = "duplicate_email", Description = "An account with the specified email already exists." }), existingUser.Id);
        }

        var user = new ApplicationUser
        {
            Email = normalizedEmail,
            UserName = normalizedEmail,
            CustomerId = string.IsNullOrWhiteSpace(customerId) ? "demo-tenant" : customerId.Trim()
        };

        var createResult = await _userManager.CreateAsync(user, password).ConfigureAwait(false);
        if (!createResult.Succeeded)
        {
            return (createResult, null);
        }

        if (roles is not null)
        {
            foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!await _roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(role)).ConfigureAwait(false);
                    if (!roleResult.Succeeded)
                    {
                        return (roleResult, user.Id);
                    }
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
                if (!addRoleResult.Succeeded)
                {
                    return (addRoleResult, user.Id);
                }
            }
        }

        if (claims is not null && claims.Count > 0)
        {
            var claimResults = await _userManager
                .AddClaimsAsync(user, claims.Select(pair => new Claim(pair.Key, pair.Value)))
                .ConfigureAwait(false);

            if (!claimResults.Succeeded)
            {
                return (claimResults, user.Id);
            }
        }

        return (IdentityResult.Success, user.Id);
    }

    /// <inheritdoc />
    public async Task<string?> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        return await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(token))
        {
            return IdentityResult.Failed(new IdentityError { Code = "invalid_token", Description = "Confirmation token is required." });
        }

        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return IdentityResult.Failed(UserNotFoundError(userId));
        }

        return await _userManager.ConfirmEmailAsync(user, token).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<string?> GeneratePasswordResetTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        return await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> ResetPasswordAsync(Guid userId, string token, string newPassword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(token))
        {
            return IdentityResult.Failed(new IdentityError { Code = "invalid_token", Description = "Reset token is required." });
        }

        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return IdentityResult.Failed(UserNotFoundError(userId));
        }

        return await _userManager.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return IdentityResult.Failed(UserNotFoundError(userId));
        }

        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Guid?> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var user = await _userManager.FindByEmailAsync(email.Trim()).ConfigureAwait(false);
        return user?.Id;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return Array.Empty<string>();
        }

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        return roles.ToArray();
    }

    /// <inheritdoc />
    public async Task<IdentityResult> AddToRolesAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return IdentityResult.Failed(UserNotFoundError(userId));
        }

        var roleList = roles.Where(role => !string.IsNullOrWhiteSpace(role)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        foreach (var role in roleList)
        {
            if (!await _roleManager.RoleExistsAsync(role).ConfigureAwait(false))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(role)).ConfigureAwait(false);
                if (!roleResult.Succeeded)
                {
                    return roleResult;
                }
            }
        }

        return await _userManager.AddToRolesAsync(user, roleList).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> RemoveFromRolesAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return IdentityResult.Failed(UserNotFoundError(userId));
        }

        var roleList = roles.Where(role => !string.IsNullOrWhiteSpace(role)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (roleList.Length == 0)
        {
            return IdentityResult.Success;
        }

        return await _userManager.RemoveFromRolesAsync(user, roleList).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Claim>> GetClaimsAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return Array.Empty<Claim>();
        }

        var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
        return claims.ToArray();
    }

    /// <inheritdoc />
    public async Task<IdentityResult> AddClaimsAsync(Guid userId, IDictionary<string, string> claims, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return IdentityResult.Failed(UserNotFoundError(userId));
        }

        if (claims.Count == 0)
        {
            return IdentityResult.Success;
        }

        var claimList = claims
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Key))
            .Select(pair => new Claim(pair.Key, pair.Value))
            .ToArray();

        return await _userManager.AddClaimsAsync(user, claimList).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> RemoveClaimsAsync(Guid userId, IDictionary<string, string> claims, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return IdentityResult.Failed(UserNotFoundError(userId));
        }

        if (claims.Count == 0)
        {
            return IdentityResult.Success;
        }

        var claimList = claims
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Key))
            .Select(pair => new Claim(pair.Key, pair.Value))
            .ToArray();

        return await _userManager.RemoveClaimsAsync(user, claimList).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> SetLockoutAsync(Guid userId, bool enabled, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await FindUserAsync(userId).ConfigureAwait(false);
        if (user is null)
        {
            return IdentityResult.Failed(UserNotFoundError(userId));
        }

        var enableResult = await _userManager.SetLockoutEnabledAsync(user, enabled).ConfigureAwait(false);
        if (!enableResult.Succeeded)
        {
            return enableResult;
        }

        if (!enabled)
        {
            return await _userManager.SetLockoutEndDateAsync(user, null).ConfigureAwait(false);
        }

        return await _userManager.SetLockoutEndDateAsync(user, lockoutEnd).ConfigureAwait(false);
    }

    private async Task<IReadOnlyCollection<Claim>> BuildClaimsAsync(ApplicationUser user, string customerClaimName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(customerClaimName, string.IsNullOrWhiteSpace(user.CustomerId) ? "demo-tenant" : user.CustomerId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
        claims.AddRange(userClaims);

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private Task<ApplicationUser?> FindUserAsync(Guid userId)
    {
        return _userManager.FindByIdAsync(userId.ToString());
    }

    private static IdentityError UserNotFoundError(Guid userId)
    {
        return new IdentityError
        {
            Code = "user_not_found",
            Description = $"User '{userId}' was not found."
        };
    }
}
