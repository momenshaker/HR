using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Services;
using HR.Infrastructure.Options;
using HR.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Infrastructure.Extensions;

/// <summary>
///     Extension methods for configuring infrastructure and application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers application services and repository implementations with the provided service collection.
    /// </summary>
    public static IServiceCollection AddHrPlatform(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new HrPlatformOptions();
        var hrPlatformSection = configuration.GetSection(HrPlatformOptions.SectionName);
        hrPlatformSection.Bind(options);

        services.Configure<HrPlatformOptions>(hrPlatformSection);

        if (!string.Equals(
                options.Data.RepositoryProvider,
                HrPlatformOptions.DataOptions.RepositoryProviders.InMemory,
                StringComparison.OrdinalIgnoreCase))
        {
            _ = DatabaseConfiguration.From(options.Data.Database, configuration);

            throw new NotSupportedException(
                $"The configured repository provider '{options.Data.RepositoryProvider}' is not supported."
            );
        }

        services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();
        services.AddSingleton<IDepartmentRepository, InMemoryDepartmentRepository>();
        services.AddSingleton<IAttendanceRecordRepository, InMemoryAttendanceRecordRepository>();
        services.AddSingleton<ILeaveRequestRepository, InMemoryLeaveRequestRepository>();
        services.AddSingleton<IPayrollRunRepository, InMemoryPayrollRunRepository>();
        services.AddSingleton<IPerformanceReviewRepository, InMemoryPerformanceReviewRepository>();
        services.AddSingleton<ICandidateRepository, InMemoryCandidateRepository>();
        services.AddSingleton<ITrainingCourseRepository, InMemoryTrainingCourseRepository>();
        services.AddSingleton<IAnnouncementRepository, InMemoryAnnouncementRepository>();
        services.AddSingleton<IAnalyticsSnapshotRepository, InMemoryAnalyticsSnapshotRepository>();

        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<ILeaveManagementService, LeaveManagementService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IPerformanceManagementService, PerformanceManagementService>();
        services.AddScoped<IRecruitmentService, RecruitmentService>();
        services.AddScoped<ITrainingService, TrainingService>();
        services.AddScoped<ICommunicationService, CommunicationService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        return services;
    }
}
