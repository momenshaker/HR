using HR.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore.Design;

namespace HR.Api.Infrastructure;

/// <summary>
///     Provides a design-time factory for <see cref="HrDbContext" /> so EF Core tooling can
///     create migrations from the API project without additional configuration switches.
/// </summary>
public sealed class HrDesignTimeDbContextFactory : IDesignTimeDbContextFactory<HrDbContext>
{
    /// <inheritdoc />
    public HrDbContext CreateDbContext(string[] args)
    {
        var factory = new HrDbContextFactory();

        return factory.CreateDbContext(args);
    }
}
