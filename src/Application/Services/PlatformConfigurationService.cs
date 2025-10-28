using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.Extensions.Options;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class PlatformConfigurationService : IPlatformConfigurationService
{
    private readonly IOptionsSnapshot<HrPlatformOptions> _optionsSnapshot;

    public PlatformConfigurationService(IOptionsSnapshot<HrPlatformOptions> optionsSnapshot)
    {
        _optionsSnapshot = optionsSnapshot;
    }

    /// <inheritdoc />
    public Task<PlatformConfigurationDto> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        var options = _optionsSnapshot.Value;
        var repositoryProvider = options.Data?.RepositoryProvider ?? HrPlatformOptions.DataOptions.RepositoryProviders.InMemory;
        var databaseProvider = ResolveDatabaseProvider(options, repositoryProvider);

        var features = Enum.GetValues<HrFeature>()
            .Select(CreateFeatureToggleStatus(options))
            .OrderBy(feature => feature.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var configuration = new PlatformConfigurationDto(repositoryProvider, databaseProvider, features);
        return Task.FromResult(configuration);
    }

    private static Func<HrFeature, FeatureToggleStatusDto> CreateFeatureToggleStatus(HrPlatformOptions options)
    {
        var featureToggles = options.Features ?? new HrPlatformOptions.FeatureToggleOptions();

        return feature =>
        {
            var metadata = FeatureMetadata[feature];
            var enabled = featureToggles.IsEnabled(feature);
            return new FeatureToggleStatusDto(metadata.FeatureKey, metadata.DisplayName, metadata.Usage, enabled);
        };
    }

    private static string ResolveDatabaseProvider(HrPlatformOptions options, string repositoryProvider)
    {
        if (!string.Equals(repositoryProvider, HrPlatformOptions.DataOptions.RepositoryProviders.EntityFrameworkCore, StringComparison.OrdinalIgnoreCase))
        {
            return "N/A";
        }

        return options.Data?.Database?.Provider ?? HrPlatformOptions.DataOptions.DatabaseOptions.Providers.SqlServer;
    }

    private static IReadOnlyDictionary<HrFeature, FeatureDescriptor> FeatureMetadata { get; } = new Dictionary<HrFeature, FeatureDescriptor>
    {
        [HrFeature.EmployeeManagement] = new("EmployeeManagement", "Employee Management", "Centralised employee profiles, lifecycle tracking, and compliance data."),
        [HrFeature.OrganizationStructure] = new("OrganizationStructure", "Organization Structure", "Department hierarchies, reporting lines, and position modelling."),
        [HrFeature.AttendanceAndTimeTracking] = new("AttendanceAndTimeTracking", "Attendance & Time Tracking", "Shift scheduling, clocking, and overtime analytics."),
        [HrFeature.LeaveManagement] = new("LeaveManagement", "Leave Management", "Configurable leave policies, balances, and approvals."),
        [HrFeature.PayrollManagement] = new("PayrollManagement", "Payroll Management", "Gross-to-net payroll processing and statutory compliance."),
        [HrFeature.PerformanceManagement] = new("PerformanceManagement", "Performance Management", "Goal tracking, KPI reviews, and appraisal cycles."),
        [HrFeature.RecruitmentAndAts] = new("RecruitmentAndAts", "Recruitment & ATS", "Candidate pipelines, interview scheduling, and offer workflows."),
        [HrFeature.TrainingAndDevelopment] = new("TrainingAndDevelopment", "Training & Development", "Course catalogues, enrolment, and learning analytics."),
        [HrFeature.InternalCommunication] = new("InternalCommunication", "Internal Communication", "Announcements, engagement campaigns, and recognition tools."),
        [HrFeature.HrAnalytics] = new("HrAnalytics", "HR Analytics", "Executive dashboards, predictive insights, and compliance reporting."),
        [HrFeature.DelegatedAuthority] = new("DelegatedAuthority", "Delegated Authority", "Delegation frameworks, approval limits, and succession coverage."),
        [HrFeature.SelfService] = new("SelfService", "Employee Self-Service", "Digital employee portals, OAuth account management, and delegated access."),
    };

    private sealed record FeatureDescriptor(string FeatureKey, string DisplayName, string Usage);
}
