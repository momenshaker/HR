namespace HR.Api.Contracts;

/// <summary>
///     Standard response payload returned after successful authentication.
/// </summary>
public sealed record AuthResponse(string AccessToken, string TokenType, int ExpiresIn, string? RefreshToken = null);
