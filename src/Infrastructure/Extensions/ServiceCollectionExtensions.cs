using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Services;
using HR.Infrastructure.Options;
using HR.Infrastructure.Persistence.EntityFramework;
using HR.Infrastructure.Persistence.EntityFramework.Repositories;
using HR.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataOptions = HR.Infrastructure.Options.HrPlatformOptions.DataOptions;

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

        var dataOptions = options.Data ?? new DataOptions();
        var repositoryProvider = dataOptions.RepositoryProvider ?? DataOptions.RepositoryProviders.InMemory;

        if (string.Equals(repositoryProvider, DataOptions.RepositoryProviders.InMemory, StringComparison.OrdinalIgnoreCase))
        {
            RegisterInMemoryRepositories(services);
        }
        else if (string.Equals(repositoryProvider, DataOptions.RepositoryProviders.EntityFrameworkCore, StringComparison.OrdinalIgnoreCase))
        {
            ConfigureEntityFramework(services, configuration, dataOptions);
        }
        else
        {
            throw new NotSupportedException(
                $"The configured repository provider '{repositoryProvider}' is not supported."
            );
        }

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

    private static void ConfigureEntityFramework(
        IServiceCollection services,
        IConfiguration configuration,
        DataOptions dataOptions)
    {
        var databaseOptions = dataOptions.Database ?? new DataOptions.DatabaseOptions();
        var databaseConfiguration = DatabaseConfiguration.From(databaseOptions, configuration);

        services.AddDbContext<HrDbContext>(optionsBuilder =>
        {
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
                case DataOptions.DatabaseOptions.Providers.SqlServer:
                    optionsBuilder.UseSqlServer(databaseConfiguration.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(HrDbContext).Assembly.FullName);
                    });
                    break;
                case DataOptions.DatabaseOptions.Providers.PostgreSql:
                    optionsBuilder.UseNpgsql(databaseConfiguration.ConnectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(HrDbContext).Assembly.FullName);
                    });
                    break;
                default:
                    throw new NotSupportedException(
                        $"The configured database provider '{databaseConfiguration.Provider}' is not supported."
                    );
            }
        });

        RegisterEntityFrameworkRepositories(services);
    }

    private static void RegisterEntityFrameworkRepositories(IServiceCollection services)
    {
        services.AddScoped<IEmployeeRepository, EntityFrameworkEmployeeRepository>();
        services.AddScoped<IDepartmentRepository, EntityFrameworkDepartmentRepository>();
        services.AddScoped<IAttendanceRecordRepository, EntityFrameworkAttendanceRecordRepository>();
        services.AddScoped<ILeaveRequestRepository, EntityFrameworkLeaveRequestRepository>();
        services.AddScoped<IPayrollRunRepository, EntityFrameworkPayrollRunRepository>();
        services.AddScoped<IPerformanceReviewRepository, EntityFrameworkPerformanceReviewRepository>();
        services.AddScoped<ICandidateRepository, EntityFrameworkCandidateRepository>();
        services.AddScoped<ITrainingCourseRepository, EntityFrameworkTrainingCourseRepository>();
        services.AddScoped<IAnnouncementRepository, EntityFrameworkAnnouncementRepository>();
        services.AddScoped<IAnalyticsSnapshotRepository, EntityFrameworkAnalyticsSnapshotRepository>();
    }

    private static void RegisterInMemoryRepositories(IServiceCollection services)
    {
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
    }
}
