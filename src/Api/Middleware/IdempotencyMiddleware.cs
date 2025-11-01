using System.Linq;
using System.Security.Claims;
using HR.Api.Contracts;
using HR.Api.Idempotency;
using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HR.Api.Middleware;

/// <summary>
///     Middleware that enforces the Idempotency-Key header on POST endpoints and replays cached responses when applicable.
/// </summary>
public sealed class IdempotencyMiddleware
{
    private const string HeaderName = "Idempotency-Key";
    private const string ReplayHeader = "Idempotency-Replayed";
    private readonly RequestDelegate _next;
    private readonly IIdempotencyStore _store;
    private readonly ILogger<IdempotencyMiddleware> _logger;
    private readonly IdempotencyOptions _options;

    public IdempotencyMiddleware(
        RequestDelegate next,
        IIdempotencyStore store,
        IOptions<IdempotencyOptions> options,
        ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;
        _store = store;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var keyValues) || string.IsNullOrWhiteSpace(keyValues))
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "missing_idempotency_key", "Idempotency-Key header is required for POST requests.").ConfigureAwait(false);
            return;
        }

        var key = BuildCacheKey(context, keyValues!);
        var cachedResponse = await _store.GetAsync(key, context.RequestAborted).ConfigureAwait(false);

        if (cachedResponse is not null)
        {
            context.Response.StatusCode = cachedResponse.StatusCode;
            if (!string.IsNullOrEmpty(cachedResponse.ContentType))
            {
                context.Response.ContentType = cachedResponse.ContentType;
            }

            foreach (var header in cachedResponse.Headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }

            context.Response.Headers[ReplayHeader] = "true";
            await context.Response.Body.WriteAsync(cachedResponse.Body, context.RequestAborted).ConfigureAwait(false);
            return;
        }

        var originalBodyStream = context.Response.Body;
        await using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        await _next(context).ConfigureAwait(false);

        responseStream.Seek(0, SeekOrigin.Begin);
        var bodyBytes = await ReadAllBytesAsync(responseStream, context.RequestAborted).ConfigureAwait(false);
        await originalBodyStream.WriteAsync(bodyBytes, context.RequestAborted).ConfigureAwait(false);

        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 400)
        {
            var headers = context.Response.Headers
                .Where(h => !string.Equals(h.Key, "Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    header => header.Key,
                    header => header.Value.Select(v => v ?? string.Empty).ToArray(),
                    StringComparer.OrdinalIgnoreCase);

            var storedResponse = new StoredIdempotentResponse
            {
                StatusCode = context.Response.StatusCode,
                ContentType = context.Response.ContentType,
                Body = bodyBytes,
                Headers = headers
            };

            var ttl = TimeSpan.FromHours(_options.WindowHours);
            await _store.SaveAsync(key, storedResponse, ttl, context.RequestAborted).ConfigureAwait(false);

            _logger.LogInformation(
                "Cached idempotent response for {Key} with status {StatusCode} and TTL {Ttl}.",
                key,
                storedResponse.StatusCode,
                ttl
            );
        }

        context.Response.Body = originalBodyStream;
    }

    private static async Task<byte[]> ReadAllBytesAsync(Stream stream, CancellationToken cancellationToken)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        return memoryStream.ToArray();
    }

    private static string BuildCacheKey(HttpContext context, string headerValue)
    {
        var actor = context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? context.User?.FindFirst("sub")?.Value
                    ?? "anonymous";

        return $"{actor}:{headerValue}";
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var traceId = context.TraceIdentifier;
        var payload = new ErrorResponse(code, message, traceId)
        {
            Details = Array.Empty<ErrorDetail>()
        };
        await context.Response.WriteAsJsonAsync(payload, cancellationToken: context.RequestAborted).ConfigureAwait(false);
    }
}
