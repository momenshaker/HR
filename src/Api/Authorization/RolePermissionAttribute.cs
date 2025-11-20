using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HR.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RolePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private static readonly IReadOnlyCollection<string> EmptyRoles = Array.Empty<string>();

    private readonly IReadOnlyCollection<string> _readRoles;
    private readonly IReadOnlyCollection<string> _writeRoles;

    public RolePermissionAttribute(string resource, string[]? readRoles = null, string[]? writeRoles = null)
    {
        ArgumentNullException.ThrowIfNull(resource);
        Resource = resource;
        _readRoles = (readRoles ?? Array.Empty<string>()).Append("Admin").Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        _writeRoles = (writeRoles ?? Array.Empty<string>()).Append("Admin").Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public string Resource { get; }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return Task.CompletedTask;
        }

        var method = context.HttpContext.Request.Method.ToUpperInvariant();
        var allowedRoles = method switch
        {
            "GET" or "HEAD" or "OPTIONS" => _readRoles,
            "POST" or "PUT" or "PATCH" or "DELETE" => _writeRoles,
            _ => _writeRoles
        };

        if (allowedRoles.Count == 0)
        {
            return Task.CompletedTask;
        }

        var user = context.HttpContext.User;
        if (allowedRoles.Any(role => user.IsInRole(role)))
        {
            return Task.CompletedTask;
        }

        context.Result = new ForbidResult();
        return Task.CompletedTask;
    }
}
