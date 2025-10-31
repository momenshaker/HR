using HR.Application.Abstractions.Repositories;
using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace HR.Api.Authorization;

/// <summary>
///     Authorization handler that restricts access to routes scoped by organization/department to an employee's org(s).
/// </summary>
public sealed class OrgGuardHandler : AuthorizationHandler<OrgGuardRequirement>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeDepartmentRepository _employeeDepartmentRepository;
    private readonly IOptionsMonitor<JwtOptions> _jwtOptions;

    public OrgGuardHandler(
        IEmployeeDepartmentRepository employeeDepartmentRepository,
        IDepartmentRepository departmentRepository,
        IOptionsMonitor<JwtOptions> jwtOptions)
    {
        _employeeDepartmentRepository = employeeDepartmentRepository ?? throw new ArgumentNullException(nameof(employeeDepartmentRepository));
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrgGuardRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            // Unknown resource type; allow other handlers/policies to decide.
            context.Succeed(requirement);
            return;
        }

        // Identify the route organization scope if present
        var routeValues = httpContext.GetRouteData()?.Values;
        if (routeValues is null || routeValues.Count == 0)
        {
            context.Succeed(requirement);
            return;
        }

        // If no org-scoped parameter is on the route, this policy is a no-op
        Guid? requiredOrganizationId = null;

        if (routeValues.TryGetValue("organizationId", out var orgRouteVal) && Guid.TryParse(orgRouteVal?.ToString(), out var orgId))
        {
            requiredOrganizationId = orgId;
        }
        else if (routeValues.TryGetValue("departmentId", out var deptRouteVal) && Guid.TryParse(deptRouteVal?.ToString(), out var departmentId))
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId, httpContext.RequestAborted).ConfigureAwait(false);
            if (department is not null)
            {
                requiredOrganizationId = department.OrganizationId;
            }
        }

        if (requiredOrganizationId is null)
        {
            // No org context — allow
            context.Succeed(requirement);
            return;
        }

        // Get employee id from configured claim
        var employeeClaimName = _jwtOptions.CurrentValue.EmployeeIdClaim;
        var employeeIdValue = context.User.FindFirst(employeeClaimName)?.Value;

        if (!Guid.TryParse(employeeIdValue, out var employeeId))
        {
            context.Fail();
            return;
        }

        // Fetch employee's departments -> map to org ids
        var departmentIds = await _employeeDepartmentRepository
            .GetDepartmentIdsByEmployeeAsync(employeeId, httpContext.RequestAborted)
            .ConfigureAwait(false);

        if (departmentIds.Count == 0)
        {
            context.Fail();
            return;
        }

        var departments = await _departmentRepository
            .GetByIdsAsync(departmentIds, httpContext.RequestAborted)
            .ConfigureAwait(false);

        var employeeOrgIds = departments.Select(d => d.OrganizationId).ToHashSet();
        if (employeeOrgIds.Contains(requiredOrganizationId.Value))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}

