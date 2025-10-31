using System.Security.Claims;

namespace HR.Application.Abstractions.Services;

public interface IJwtTokenService
{
    string CreateAccessToken(IEnumerable<Claim> claims);
    (string Token, DateTime ExpiresAtUtc) CreateRefreshToken();
}
