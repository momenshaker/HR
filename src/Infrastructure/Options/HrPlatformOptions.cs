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
        public string RepositoryProvider { get; set; } = "InMemory";
    }
}
