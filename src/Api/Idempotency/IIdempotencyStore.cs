namespace HR.Api.Idempotency;

/// <summary>
///     Provides storage capabilities for idempotent API responses.
/// </summary>
public interface IIdempotencyStore
{
    Task<StoredIdempotentResponse?> GetAsync(string key, CancellationToken cancellationToken);

    Task SaveAsync(string key, StoredIdempotentResponse response, TimeSpan ttl, CancellationToken cancellationToken);
}
