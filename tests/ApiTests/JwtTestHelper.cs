using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HR.Api.IntegrationTests;

internal static class JwtTestHelper
{
    public static string CreateToken(string issuer, string audience, string signingKey, IEnumerable<string> roles, IDictionary<string, string>? additionalClaims = null)
    {
        var now = DateTime.UtcNow;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new("cust", "test-tenant")
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        if (additionalClaims is not null)
        {
            claims.AddRange(additionalClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));
        }

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            now,
            now.AddHours(1),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
