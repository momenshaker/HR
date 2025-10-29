namespace HR.Api.Contracts;

/// <summary>
///     Represents credentials submitted to obtain an access token.
/// </summary>
public sealed record LoginRequest(string Email, string Password);
