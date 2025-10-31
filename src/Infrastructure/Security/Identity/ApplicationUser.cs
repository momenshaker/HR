using Microsoft.AspNetCore.Identity;

namespace HR.Infrastructure.Security.Identity;

/// <summary>
///     Custom identity user for the HR platform to support tenant scoping and auditing.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    ///     Gets or sets the tenant or customer identifier associated with the account.
    /// </summary>
    public string CustomerId { get; set; } = "demo-tenant";

    /// <summary>
    ///     Gets or sets the linked employee identifier when the account represents an employee.
    /// </summary>
    public Guid? EmployeeId { get; set; }
}
