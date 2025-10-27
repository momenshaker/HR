using HR.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HR.Infrastructure.Persistence.EntityFramework;

/// <summary>
///     Design-time factory used by Entity Framework Core tools for creating <see cref="HrDbContext" /> instances.
/// </summary>
public sealed class HrDbContextFactory : IDesignTimeDbContextFactory<HrDbContext>
{
    /// <inheritdoc />
    public HrDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var databaseOptions = new HrPlatformOptions.DataOptions.DatabaseOptions();
        configuration
            .GetSection($"{HrPlatformOptions.SectionName}:Data:Database")
            .Bind(databaseOptions);

        var databaseConfiguration = DatabaseConfiguration.From(databaseOptions, configuration);

        var optionsBuilder = new DbContextOptionsBuilder<HrDbContext>();

        if (databaseConfiguration.EnableDetailedErrors)
        {
            optionsBuilder.EnableDetailedErrors();
        }

        if (databaseConfiguration.EnableSensitiveDataLogging)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        switch (databaseConfiguration.Provider)
        {
            case HrPlatformOptions.DataOptions.DatabaseOptions.Providers.SqlServer:
                optionsBuilder.UseSqlServer(databaseConfiguration.ConnectionString);
                break;
            case HrPlatformOptions.DataOptions.DatabaseOptions.Providers.PostgreSql:
                optionsBuilder.UseNpgsql(databaseConfiguration.ConnectionString);
                break;
            default:
                throw new NotSupportedException(
                    $"The configured database provider '{databaseConfiguration.Provider}' is not supported."
                );
        }

        return new HrDbContext(optionsBuilder.Options);
    }
}
