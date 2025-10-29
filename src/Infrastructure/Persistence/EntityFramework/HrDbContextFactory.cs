using System.IO;
using HR.Application.Configuration;
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
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var databaseOptions = new HrPlatformOptions.DataOptions.DatabaseOptions();
        configuration
            .GetSection($"{HrPlatformOptions.SectionName}:Data:Database")
            .Bind(databaseOptions);

        if (string.IsNullOrWhiteSpace(databaseOptions.ConnectionString)
            && string.IsNullOrWhiteSpace(databaseOptions.ConnectionStringName))
        {
            var sqliteDatabasePath = Path.Combine(AppContext.BaseDirectory, "hr-development.sqlite");

            databaseOptions.Provider = HrPlatformOptions.DataOptions.DatabaseOptions.Providers.Sqlite;
            databaseOptions.ConnectionString = $"Data Source={sqliteDatabasePath}";
        }

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

        var migrationsAssemblyName = typeof(HrDbContext).Assembly.GetName().Name;

        switch (databaseConfiguration.Provider)
        {
            case HrPlatformOptions.DataOptions.DatabaseOptions.Providers.SqlServer:
                optionsBuilder.UseSqlServer(
                    databaseConfiguration.ConnectionString,
                    sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssemblyName)
                );
                break;
            case HrPlatformOptions.DataOptions.DatabaseOptions.Providers.PostgreSql:
                optionsBuilder.UseNpgsql(
                    databaseConfiguration.ConnectionString,
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(migrationsAssemblyName)
                );
                break;
            case HrPlatformOptions.DataOptions.DatabaseOptions.Providers.Sqlite:
                optionsBuilder.UseSqlite(
                    databaseConfiguration.ConnectionString,
                    sqliteOptions => sqliteOptions.MigrationsAssembly(migrationsAssemblyName)
                );
                break;
            default:
                throw new NotSupportedException(
                    $"The configured database provider '{databaseConfiguration.Provider}' is not supported."
                );
        }

        return new HrDbContext(optionsBuilder.Options);
    }
}
