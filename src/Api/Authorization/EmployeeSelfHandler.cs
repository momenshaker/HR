using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace HR.Api.Authorization;

public sealed class EmployeeSelfHandler : AuthorizationHandler<EmployeeSelfRequirement>
{
    private readonly IOptionsMonitor<JwtOptions> _jwtOptions;

    public EmployeeSelfHandler(IOptionsMonitor<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployeeSelfRequirement requirement)
    {
        var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (roles.Contains("Admin") || roles.Contains("Manager"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (context.Resource is HttpContext httpContext)
        {
            var routeValues = httpContext.GetRouteData()?.Values;
            if (routeValues is not null)
            {
                object? routeVal = null;
                if (!routeValues.TryGetValue("employeeId", out routeVal))
                {
                    routeValues.TryGetValue("id", out routeVal);
                }
                if (routeVal is not null && Guid.TryParse(routeVal.ToString(), out var employeeId))
                {
                var claimName = _jwtOptions.CurrentValue.EmployeeIdClaim;
                var claimValue = context.User.FindFirst(claimName)?.Value;
                if (Guid.TryParse(claimValue, out var claimEmployeeId) && claimEmployeeId == employeeId)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                }
            }
        }

        context.Fail();
        return Task.CompletedTask;
    }
}
