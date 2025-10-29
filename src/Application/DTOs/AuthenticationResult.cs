namespace HR.Application.DTOs;

/// <summary>
///     Represents the outcome of a successful authentication attempt.
/// </summary>
public sealed record AuthenticationResult(
    string AccessToken,
    TimeSpan ExpiresIn,
    string TokenType = "Bearer",
    string? RefreshToken = null
);
