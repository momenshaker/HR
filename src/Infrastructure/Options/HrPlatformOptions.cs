namespace HR.Infrastructure.Options;

/// <summary>
///     Strongly typed configuration model for HR platform settings.
/// </summary>
public sealed class HrPlatformOptions
{
    /// <summary>
    ///     Configuration section name bound from <c>appsettings.json</c>.
    /// </summary>
    public const string SectionName = "HrPlatform";

    /// <summary>
    ///     Gets or sets feature toggle configuration for individual modules.
    /// </summary>
    public FeatureToggleOptions Features { get; set; } = new();

    /// <summary>
    ///     Gets or sets data access configuration.
    /// </summary>
    public DataOptions Data { get; set; } = new();

    /// <summary>
    ///     Describes feature level configuration toggles.
    /// </summary>
    public sealed class FeatureToggleOptions
    {
        public bool EmployeeManagement { get; set; } = true;
        public bool OrganizationStructure { get; set; } = true;
        public bool AttendanceAndTimeTracking { get; set; } = true;
        public bool LeaveManagement { get; set; } = true;
        public bool PayrollManagement { get; set; } = true;
        public bool PerformanceManagement { get; set; } = true;
        public bool RecruitmentAndAts { get; set; } = true;
        public bool TrainingAndDevelopment { get; set; } = true;
        public bool InternalCommunication { get; set; } = true;
        public bool HrAnalytics { get; set; } = true;

        /// <summary>
        ///     Determines if a feature is enabled.
        /// </summary>
        /// <param name="feature">The feature to evaluate.</param>
        /// <returns><c>true</c> when enabled, otherwise <c>false</c>.</returns>
        public bool IsEnabled(HrFeature feature)
        {
            return feature switch
            {
                HrFeature.EmployeeManagement => EmployeeManagement,
                HrFeature.OrganizationStructure => OrganizationStructure,
                HrFeature.AttendanceAndTimeTracking => AttendanceAndTimeTracking,
                HrFeature.LeaveManagement => LeaveManagement,
                HrFeature.PayrollManagement => PayrollManagement,
                HrFeature.PerformanceManagement => PerformanceManagement,
                HrFeature.RecruitmentAndAts => RecruitmentAndAts,
                HrFeature.TrainingAndDevelopment => TrainingAndDevelopment,
                HrFeature.InternalCommunication => InternalCommunication,
                HrFeature.HrAnalytics => HrAnalytics,
                _ => false
            };
        }
    }

    /// <summary>
    ///     Describes repository provider configuration.
    /// </summary>
    public sealed class DataOptions
    {
        /// <summary>
        ///     Defines supported repository provider identifiers.
        /// </summary>
        public static class RepositoryProviders
        {
            /// <summary>
            ///     Represents the in-memory repository provider used for local development and testing.
            /// </summary>
            public const string InMemory = "InMemory";

            /// <summary>
            ///     Represents the Entity Framework Core provider backed by a relational database.
            /// </summary>
            public const string EntityFrameworkCore = "EntityFrameworkCore";
        }

        /// <summary>
        ///     Gets or sets the repository provider identifier.
        /// </summary>
        public string RepositoryProvider { get; set; } = RepositoryProviders.InMemory;

        /// <summary>
        ///     Gets or sets relational database specific configuration.
        /// </summary>
        public DatabaseOptions Database { get; set; } = new();

        /// <summary>
        ///     Describes relational database configuration.
        /// </summary>
        public sealed class DatabaseOptions
        {
            /// <summary>
            ///     Defines supported database provider identifiers.
            /// </summary>
            public static class Providers
            {
                /// <summary>
                ///     Represents Microsoft SQL Server provider.
                /// </summary>
                public const string SqlServer = "SqlServer";

                /// <summary>
                ///     Represents PostgreSQL provider.
                /// </summary>
                public const string PostgreSql = "PostgreSql";

                /// <summary>
                ///     Represents the SQLite provider.
                /// </summary>
                public const string Sqlite = "Sqlite";
            }

            /// <summary>
            ///     Gets or sets the database provider identifier.
            /// </summary>
            public string Provider { get; set; } = Providers.SqlServer;

            /// <summary>
            ///     Gets or sets the connection string used to connect to the database. When left empty,
            ///     <see cref="ConnectionStringName" /> will be used to resolve the value from the root
            ///     <c>ConnectionStrings</c> configuration section.
            /// </summary>
            public string ConnectionString { get; set; } = string.Empty;

            /// <summary>
            ///     Gets or sets the name of the connection string stored in the root configuration section.
            /// </summary>
            public string ConnectionStringName { get; set; } = string.Empty;

            /// <summary>
            ///     Gets or sets a value indicating whether detailed errors should be enabled for the database provider.
            /// </summary>
            public bool EnableDetailedErrors { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether sensitive data logging should be enabled.
            /// </summary>
            public bool EnableSensitiveDataLogging { get; set; }
        }
    }
}
