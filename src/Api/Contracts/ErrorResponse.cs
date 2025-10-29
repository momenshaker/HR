namespace HR.Api.Contracts;

/// <summary>
///     Standardized error payload returned by the API.
/// </summary>
public sealed record ErrorResponse(string Code, string Message, string? TraceId = null)
{
    /// <summary>
    ///     Detailed field-level errors when applicable.
    /// </summary>
    public IReadOnlyCollection<ErrorDetail> Details { get; init; } = Array.Empty<ErrorDetail>();
}

/// <summary>
///     Represents a single validation or field-level error.
/// </summary>
/// <param name="Field">The field that produced the error.</param>
/// <param name="Message">Human readable description of the failure.</param>
public sealed record ErrorDetail(string Field, string Message);
