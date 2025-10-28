using System;
using HR.Application.Configuration;
using Microsoft.Extensions.Configuration;

namespace HR.Infrastructure.Options;

/// <summary>
///     Represents resolved database configuration bound from application settings.
/// </summary>
public sealed class DatabaseConfiguration
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DatabaseConfiguration" /> class.
    /// </summary>
    /// <param name="provider">The database provider identifier.</param>
    /// <param name="connectionString">The resolved database connection string.</param>
    /// <param name="enableDetailedErrors">A value indicating whether detailed errors are enabled.</param>
    /// <param name="enableSensitiveDataLogging">A value indicating whether sensitive data logging is enabled.</param>
    public DatabaseConfiguration(
        string provider,
        string connectionString,
        bool enableDetailedErrors,
        bool enableSensitiveDataLogging)
    {
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        EnableDetailedErrors = enableDetailedErrors;
        EnableSensitiveDataLogging = enableSensitiveDataLogging;
    }

    /// <summary>
    ///     Gets the database provider identifier.
    /// </summary>
    public string Provider { get; }

    /// <summary>
    ///     Gets the resolved connection string used to connect to the database.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    ///     Gets a value indicating whether detailed errors are enabled.
    /// </summary>
    public bool EnableDetailedErrors { get; }

    /// <summary>
    ///     Gets a value indicating whether sensitive data logging is enabled.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; }

    /// <summary>
    ///     Creates a <see cref="DatabaseConfiguration" /> instance from the configured options.
    /// </summary>
    /// <param name="databaseOptions">The configured database options.</param>
    /// <param name="configuration">The application configuration used to resolve named connection strings.</param>
    /// <returns>A populated <see cref="DatabaseConfiguration" /> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when arguments are <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no connection string could be resolved.</exception>
    public static DatabaseConfiguration From(
        HrPlatformOptions.DataOptions.DatabaseOptions databaseOptions,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(databaseOptions);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = ResolveConnectionString(databaseOptions, configuration);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A database connection string must be configured when using a relational repository provider. " +
                "Provide HrPlatform:Data:Database:ConnectionString or specify " +
                "HrPlatform:Data:Database:ConnectionStringName to reference an entry under ConnectionStrings."
            );
        }

        return new DatabaseConfiguration(
            string.IsNullOrWhiteSpace(databaseOptions.Provider)
                ? HrPlatformOptions.DataOptions.DatabaseOptions.Providers.SqlServer
                : databaseOptions.Provider,
            connectionString,
            databaseOptions.EnableDetailedErrors,
            databaseOptions.EnableSensitiveDataLogging
        );
    }

    private static string ResolveConnectionString(
        HrPlatformOptions.DataOptions.DatabaseOptions databaseOptions,
        IConfiguration configuration)
    {
        if (!string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
        {
            return databaseOptions.ConnectionString;
        }

        if (!string.IsNullOrWhiteSpace(databaseOptions.ConnectionStringName))
        {
            return configuration.GetConnectionString(databaseOptions.ConnectionStringName) ?? string.Empty;
        }

        return string.Empty;
    }
}
