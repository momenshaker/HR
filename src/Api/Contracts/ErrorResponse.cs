namespace HR.Api.Contracts;

/// <summary>
///     Standardized error payload returned by the API.
/// </summary>
public sealed record ErrorResponse(string Code, string Message, string? TraceId = null);
