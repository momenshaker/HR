using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HR.Application.Abstractions.Services;
using HR.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HR.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IOptionsMonitor<JwtOptions> _jwtOptions;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtTokenService(IOptionsMonitor<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
    }

    public string CreateAccessToken(IEnumerable<Claim> claims)
    {
        var options = _jwtOptions.CurrentValue;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(options.AccessTokenMinutes),
            signingCredentials: creds);

        return _handler.WriteToken(token);
    }

    public (string Token, DateTime ExpiresAtUtc) CreateRefreshToken()
    {
        Span<byte> buffer = stackalloc byte[64];
        RandomNumberGenerator.Fill(buffer);
        var raw = Convert.ToBase64String(buffer);
        var expires = DateTime.UtcNow.AddDays(_jwtOptions.CurrentValue.RefreshTokenDays);
        return (raw, expires);
    }
}
