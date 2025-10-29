using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HR.Api.Middleware;

/// <summary>
///     Middleware responsible for generating ETag headers for GET by id endpoints and honouring If-None-Match requests.
/// </summary>
public sealed class EtagMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method) || !context.Request.RouteValues.ContainsKey("id"))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await _next(context).ConfigureAwait(false);

        if (context.Response.StatusCode != StatusCodes.Status200OK)
        {
            buffer.Seek(0, SeekOrigin.Begin);
            await buffer.CopyToAsync(originalBody, context.RequestAborted).ConfigureAwait(false);
            context.Response.Body = originalBody;
            return;
        }

        buffer.Seek(0, SeekOrigin.Begin);
        var payload = await ReadAllBytesAsync(buffer, context.RequestAborted).ConfigureAwait(false);
        var etag = ComputeEtag(payload);

        if (context.Request.Headers.TryGetValue("If-None-Match", out StringValues match) && match.Any(value => string.Equals(value, etag, StringComparison.Ordinal)))
        {
            context.Response.Body = originalBody;
            context.Response.StatusCode = StatusCodes.Status304NotModified;
            context.Response.ContentLength = 0;
            context.Response.Headers.ETag = etag;
            return;
        }

        context.Response.Headers.ETag = etag;
        context.Response.Body = originalBody;
        await context.Response.Body.WriteAsync(payload, context.RequestAborted).ConfigureAwait(false);
    }

    private static async Task<byte[]> ReadAllBytesAsync(Stream stream, CancellationToken cancellationToken)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        return memoryStream.ToArray();
    }

    private static string ComputeEtag(byte[] payload)
    {
        if (payload.Length == 0)
        {
            return "\"0\"";
        }

        var hash = SHA256.HashData(payload);
        var encoded = Convert.ToBase64String(hash);
        return $"\"{encoded}\"";
    }
}
