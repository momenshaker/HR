using System.Linq;
using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Security.Identity;

/// <summary>
///     Seeds ASP.NET Core Identity users and roles based on <see cref="AuthenticationOptions"/> configuration.
/// </summary>
public static class IdentitySeeder
{
    /// <summary>
    ///     Ensures the configured users and roles exist in the identity store.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var options = scopedProvider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Ensure baseline roles
        var baselineRoles = new[] { "Admin", "Manager", "Employee" };
        foreach (var roleName in baselineRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName)).ConfigureAwait(false);
            }
        }

        // Seed configured users
        foreach (var userOptions in options.Users)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var email = userOptions.Email.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                continue;
            }

            var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    Id = userOptions.Id == Guid.Empty ? Guid.NewGuid() : userOptions.Id,
                    Email = email,
                    UserName = email,
                    CustomerId = string.IsNullOrWhiteSpace(userOptions.CustomerId) ? "demo-tenant" : userOptions.CustomerId,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, userOptions.Password).ConfigureAwait(false);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create identity user '{email}': {string.Join(", ", createResult.Errors.Select(error => error.Description))}."
                    );
                }
            }

            if (userOptions.Roles is not null)
            {
                foreach (var roleName in userOptions.Roles)
                {
                    if (string.IsNullOrWhiteSpace(roleName))
                    {
                        continue;
                    }

                    if (!await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
                    {
                        var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName)).ConfigureAwait(false);
                        if (!roleResult.Succeeded)
                        {
                            throw new InvalidOperationException(
                                $"Failed to create identity role '{roleName}': {string.Join(", ", roleResult.Errors.Select(error => error.Description))}."
                            );
                        }
                    }

                    if (!await userManager.IsInRoleAsync(user, roleName).ConfigureAwait(false))
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(user, roleName).ConfigureAwait(false);
                        if (!addRoleResult.Succeeded)
                        {
                            throw new InvalidOperationException(
                                $"Failed to assign role '{roleName}' to '{email}': {string.Join(", ", addRoleResult.Errors.Select(error => error.Description))}."
                            );
                        }
                    }
                }
            }

            if (userOptions.Claims is not null && userOptions.Claims.Count > 0)
            {
                var existingClaims = await userManager.GetClaimsAsync(user).ConfigureAwait(false);

                foreach (var claim in userOptions.Claims)
                {
                    if (existingClaims.Any(existing => existing.Type == claim.Key && existing.Value == claim.Value))
                    {
                        continue;
                    }

                    var claimResult = await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(claim.Key, claim.Value))
                        .ConfigureAwait(false);
                    if (!claimResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to assign claim '{claim.Key}' to '{email}': {string.Join(", ", claimResult.Errors.Select(error => error.Description))}."
                        );
                    }
                }
            }
        }

        // Backfill a few existing employees with accounts (demo)
        var db = scopedProvider.GetRequiredService<Infrastructure.Persistence.EntityFramework.HrDbContext>();
        var demoEmployees = await db.Employees.Take(3).ToListAsync(cancellationToken).ConfigureAwait(false);
        foreach (var emp in demoEmployees)
        {
            if (await userManager.Users.AnyAsync(u => u.EmployeeId == emp.Id, cancellationToken).ConfigureAwait(false))
            {
                continue;
            }

            var username = $"{emp.FirstName}.{emp.LastName}".ToLowerInvariant();
            var email = emp.Email;
            var existing = await userManager.FindByEmailAsync(email).ConfigureAwait(false) ?? await userManager.FindByNameAsync(username).ConfigureAwait(false);
            if (existing is not null)
            {
                if (existing.EmployeeId is null)
                {
                    existing.EmployeeId = emp.Id;
                    await userManager.UpdateAsync(existing).ConfigureAwait(false);
                }
                continue;
            }

            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = username,
                EmailConfirmed = true,
                EmployeeId = emp.Id
            };
            // Note: for dev only; do not ship plain default passwords.
            var create = await userManager.CreateAsync(newUser, "DevUser!123").ConfigureAwait(false);
            if (create.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, "Employee").ConfigureAwait(false);
            }
        }
    }
}
