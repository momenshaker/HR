using System.Linq;
using System.Security.Claims;
using HR.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HR.Api.Filters;

/// <summary>
///     Logs create, update, and delete operations executed through the API controllers.
/// </summary>
public sealed class AuditLoggingFilter(IAuditLogger auditLogger) : IAsyncActionFilter
{
    private static readonly HashSet<string> AuditedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    };

    private readonly IAuditLogger _auditLogger = auditLogger;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next().ConfigureAwait(false);
        if (executedContext.Exception is not null)
        {
            return;
        }

        var requestMethod = context.HttpContext.Request.Method;
        if (!AuditedMethods.Contains(requestMethod))
        {
            return;
        }

        var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var resource = controllerActionDescriptor?.ControllerTypeInfo
            .GetCustomAttributes(typeof(AuditResourceAttribute), inherit: true)
            .OfType<AuditResourceAttribute>()
            .FirstOrDefault()?.ResourceName
            ?? controllerActionDescriptor?.ControllerName
            ?? "Unknown";

        var action = requestMethod switch
        {
            var method when string.Equals(method, HttpMethods.Post, StringComparison.OrdinalIgnoreCase) => "create",
            var method when string.Equals(method, HttpMethods.Delete, StringComparison.OrdinalIgnoreCase) => "delete",
            _ => "update"
        };

        var entityId = ResolveEntityId(context);
        var actorId = context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? context.HttpContext.User?.FindFirst("sub")?.Value
                       ?? "unknown";

        var entry = new AuditLogEntry(
            actorId,
            action,
            resource,
            entityId ?? "n/a",
            context.HttpContext.TraceIdentifier
        );

        await _auditLogger.LogAsync(entry, context.HttpContext.RequestAborted).ConfigureAwait(false);
    }

    private static string? ResolveEntityId(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("id", out var idValue) && idValue is not null)
        {
            return idValue.ToString();
        }

        return context.ActionArguments.Values.FirstOrDefault(v => v is Guid or string)?.ToString();
    }
}
