using HR.Api.Contracts;
using HR.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HR.Api.Middleware;

/// <summary>
///     Middleware that enforces subscription entitlements before allowing access to feature endpoints.
/// </summary>
public sealed class SubscriptionGuardMiddleware
{
    private const string MissingEntitlementErrorCode = "subscription_entitlement_denied";

    private readonly RequestDelegate _next;
    private readonly ILogger<SubscriptionGuardMiddleware> _logger;

    public SubscriptionGuardMiddleware(RequestDelegate next, ILogger<SubscriptionGuardMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, ISubscriptionService subscriptionService)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(subscriptionService);

        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        var entitlementAttributes = endpoint.Metadata.GetOrderedMetadata<RequiresSubscriptionEntitlementAttribute>();
        if (entitlementAttributes is null || entitlementAttributes.Count == 0)
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        foreach (var entitlement in entitlementAttributes)
        {
            var hasEntitlement = await subscriptionService
                .HasEntitlementAsync(entitlement.Feature, context.RequestAborted)
                .ConfigureAwait(false);

            if (hasEntitlement)
            {
                continue;
            }

            _logger.LogWarning("Subscription entitlement '{Feature}' denied for path {Path}", entitlement.Feature, context.Request.Path);
            await WriteForbiddenAsync(context, entitlement.Feature.ToString()).ConfigureAwait(false);
            return;
        }

        await _next(context).ConfigureAwait(false);
    }

    private static async Task WriteForbiddenAsync(HttpContext context, string feature)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        var response = new ErrorResponse(
            MissingEntitlementErrorCode,
            $"Active subscription does not include the '{feature}' entitlement.",
            context.TraceIdentifier);

        await context.Response.WriteAsJsonAsync(response, cancellationToken: context.RequestAborted).ConfigureAwait(false);
    }
}
