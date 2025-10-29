using Microsoft.Extensions.Caching.Memory;

namespace HR.Api.Idempotency;

/// <summary>
///     In-memory implementation of <see cref="IIdempotencyStore"/>.
/// </summary>
public sealed class InMemoryIdempotencyStore(IMemoryCache cache) : IIdempotencyStore
{
    private readonly IMemoryCache _cache = cache;

    public Task<StoredIdempotentResponse?> GetAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = _cache.TryGetValue(key, out StoredIdempotentResponse? value);
        return Task.FromResult(value);
    }

    public Task SaveAsync(string key, StoredIdempotentResponse response, TimeSpan ttl, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        _cache.Set(key, response, cacheEntryOptions);
        return Task.CompletedTask;
    }
}
