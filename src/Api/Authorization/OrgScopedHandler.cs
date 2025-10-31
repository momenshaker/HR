using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace HR.Api.Authorization;

public sealed class OrgScopedHandler : AuthorizationHandler<OrgScopedRequirement>
{
    private readonly IOptionsMonitor<JwtOptions> _jwtOptions;
    public OrgScopedHandler(IOptionsMonitor<JwtOptions> jwtOptions) { _jwtOptions = jwtOptions; }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrgScopedRequirement requirement)
    {
        // If user has an org claim, enforce it matches route org
        var orgClaim = context.User.FindFirst("org_id")?.Value;
        if (string.IsNullOrWhiteSpace(orgClaim))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (context.Resource is HttpContext httpContext)
        {
            var routeValues = httpContext.GetRouteData()?.Values;
            if (routeValues is not null && routeValues.TryGetValue("organizationId", out var orgVal) && Guid.TryParse(orgVal?.ToString(), out var orgId))
            {
                if (string.Equals(orgClaim, orgId.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
        }

        context.Fail();
        return Task.CompletedTask;
    }
}

