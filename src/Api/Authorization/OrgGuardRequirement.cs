using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Authorization;

/// <summary>
///     Requirement ensuring that an authenticated employee can only access resources within their organization(s).
/// </summary>
public sealed class OrgGuardRequirement : IAuthorizationRequirement
{
}

