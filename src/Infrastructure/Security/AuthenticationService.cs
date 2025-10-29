using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HR.Infrastructure.Security;

/// <summary>
///     Default authentication service that issues JWT bearer tokens using configured development accounts.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IOptionsMonitor<AuthenticationOptions> _authenticationOptions;
    private readonly IOptionsMonitor<JwtOptions> _jwtOptions;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TimeProvider _timeProvider;

    public AuthenticationService(
        IOptionsMonitor<AuthenticationOptions> authenticationOptions,
        IOptionsMonitor<JwtOptions> jwtOptions,
        TimeProvider timeProvider)
    {
        _authenticationOptions = authenticationOptions ?? throw new ArgumentNullException(nameof(authenticationOptions));
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    /// <inheritdoc />
    public Task<AuthenticationResult?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult<AuthenticationResult?>(null);
        }

        var configuredUser = _authenticationOptions.CurrentValue.Users
            .FirstOrDefault(user => string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase));

        if (configuredUser is null || !string.Equals(configuredUser.Password, password, StringComparison.Ordinal))
        {
            return Task.FromResult<AuthenticationResult?>(null);
        }

        var jwtOptions = _jwtOptions.CurrentValue;
        if (string.IsNullOrWhiteSpace(jwtOptions.Key))
        {
            throw new InvalidOperationException("JWT signing key is not configured.");
        }

        var now = _timeProvider.GetUtcNow();
        var expires = now.AddMinutes(_authenticationOptions.CurrentValue.TokenLifetimeMinutes);

        var claims = BuildClaims(configuredUser, jwtOptions.CustomerClaim);
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

        var result = new AuthenticationResult(accessToken, expires - now, "Bearer", null);
        return Task.FromResult<AuthenticationResult?>(result);
    }

    private static IEnumerable<Claim> BuildClaims(AuthenticationOptions.UserOptions user, string customerClaimName)
    {
        var identifier = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, identifier.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, identifier.ToString()),
            new(customerClaimName, string.IsNullOrWhiteSpace(user.CustomerId) ? "demo-tenant" : user.CustomerId)
        };

        if (user.Roles is not null && user.Roles.Count > 0)
        {
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        if (user.Claims is not null && user.Claims.Count > 0)
        {
            claims.AddRange(user.Claims.Select(claim => new Claim(claim.Key, claim.Value)));
        }

        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        return claims;
    }
}
