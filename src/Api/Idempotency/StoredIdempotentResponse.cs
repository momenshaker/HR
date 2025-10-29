namespace HR.Api.Idempotency;

/// <summary>
///     Represents the cached response returned for repeated idempotent requests.
/// </summary>
public sealed class StoredIdempotentResponse
{
    public int StatusCode { get; init; }

    public string? ContentType { get; init; }

    public byte[] Body { get; init; } = Array.Empty<byte>();

    public IReadOnlyDictionary<string, string[]> Headers { get; init; } = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
}
