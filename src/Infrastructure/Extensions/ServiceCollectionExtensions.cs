using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.Services;
using HR.Infrastructure.Logging;
using HR.Infrastructure.Options;
using HR.Infrastructure.Persistence.EntityFramework;
using HR.Infrastructure.Persistence.EntityFramework.Repositories;
using HR.Infrastructure.Persistence.Repositories;
using HR.Infrastructure.Security;
using HR.Infrastructure.Security.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataOptions = HR.Application.Configuration.HrPlatformOptions.DataOptions;

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
        services.Configure<StripeWebhookOptions>(configuration.GetSection(StripeWebhookOptions.SectionName));

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

        ConfigureIdentityServices(services);

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployeeSearchService, EmployeeService>();
        services.AddScoped<IEmployeeDepartmentService, EmployeeDepartmentService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDepartmentTreeService, DepartmentTreeService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IOrganizationUnitService, OrganizationUnitService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IReportingRelationshipService, ReportingRelationshipService>();
        services.AddScoped<IDelegatedAuthorityService, DelegatedAuthorityService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<ILeaveManagementService, LeaveManagementService>();
        services.AddSingleton<IWorkdayCalendar, DefaultWorkdayCalendar>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IPerformanceManagementService, PerformanceManagementService>();
        services.AddScoped<IRecruitmentService, RecruitmentService>();
        services.AddScoped<ITrainingService, TrainingService>();
        services.AddScoped<IEmployeeSelfService, EmployeeSelfService>();
        services.AddScoped<ISelfServiceAccountService, SelfServiceAccountService>();
        services.AddScoped<ICommunicationService, CommunicationService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<ILeaveRolloverService, LeaveRolloverService>();
        services.AddScoped<IPlatformConfigurationService, PlatformConfigurationService>();
        services.AddScoped<ITimesheetService, TimesheetService>();
        services.AddSingleton<ISubscriptionService, SubscriptionService>();
        services.AddSingleton<IInvoiceService, InvoiceService>();
        services.AddSingleton<IUsageService, UsageService>();

        services.AddScoped<ISubscriptionBillingService, SubscriptionBillingService>();
        services.AddScoped<IAuditLogger, AuditLogger>();

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
                case DataOptions.DatabaseOptions.Providers.Sqlite:
                    optionsBuilder.UseSqlite(databaseConfiguration.ConnectionString, sqliteOptions =>
                    {
                        sqliteOptions.MigrationsAssembly(typeof(HrDbContext).Assembly.FullName);
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
        services.AddScoped<IEmployeeDepartmentRepository, EntityFrameworkEmployeeDepartmentRepository>();
        services.AddScoped<IDepartmentRepository, EntityFrameworkDepartmentRepository>();
        services.AddScoped<IOrganizationRepository, EntityFrameworkOrganizationRepository>();
        services.AddScoped<IOrganizationUnitRepository, EntityFrameworkOrganizationUnitRepository>();
        services.AddScoped<IPositionRepository, EntityFrameworkPositionRepository>();
        services.AddScoped<IReportingRelationshipRepository, EntityFrameworkReportingRelationshipRepository>();
        services.AddScoped<IDelegatedAuthorityRepository, EntityFrameworkDelegatedAuthorityRepository>();
        services.AddScoped<IAttendanceRecordRepository, EntityFrameworkAttendanceRecordRepository>();
        services.AddScoped<ILeaveRequestRepository, EntityFrameworkLeaveRequestRepository>();
        services.AddScoped<IPayrollRunRepository, EntityFrameworkPayrollRunRepository>();
        services.AddScoped<IPerformanceReviewRepository, EntityFrameworkPerformanceReviewRepository>();
        services.AddScoped<ICandidateRepository, EntityFrameworkCandidateRepository>();
        services.AddScoped<ITrainingCourseRepository, EntityFrameworkTrainingCourseRepository>();
        services.AddScoped<ICourseEnrollmentRepository, EntityFrameworkCourseEnrollmentRepository>();
        services.AddScoped<ICourseCertificationRepository, EntityFrameworkCourseCertificationRepository>();
        services.AddScoped<IAnnouncementRepository, EntityFrameworkAnnouncementRepository>();
        services.AddScoped<IEngagementCampaignRepository, EntityFrameworkEngagementCampaignRepository>();
        services.AddScoped<IPulseSurveyRepository, EntityFrameworkPulseSurveyRepository>();
        services.AddScoped<IRecognitionProgramRepository, EntityFrameworkRecognitionProgramRepository>();
        services.AddScoped<IAnalyticsSnapshotRepository, EntityFrameworkAnalyticsSnapshotRepository>();
        services.AddScoped<ILeaveTypeRepository, EntityFrameworkLeaveTypeRepository>();
        services.AddScoped<ILeaveBalanceRepository, EntityFrameworkLeaveBalanceRepository>();
        services.AddScoped<IVacancyRepository, EntityFrameworkVacancyRepository>();
        services.AddScoped<IInterviewScheduleRepository, EntityFrameworkInterviewScheduleRepository>();
        services.AddScoped<ISelfServiceAccountRepository, EntityFrameworkSelfServiceAccountRepository>();
        services.AddScoped<ITimesheetRepository, EntityFrameworkTimesheetRepository>();
    }

    private static void RegisterInMemoryRepositories(IServiceCollection services)
    {
        services.AddDbContext<HrDbContext>(options => options.UseInMemoryDatabase("hr-platform-identity"));

        services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();
        services.AddSingleton<IEmployeeDepartmentRepository, InMemoryEmployeeDepartmentRepository>();
        services.AddSingleton<IDepartmentRepository, InMemoryDepartmentRepository>();
        services.AddSingleton<IOrganizationRepository, InMemoryOrganizationRepository>();
        services.AddSingleton<IOrganizationUnitRepository, InMemoryOrganizationUnitRepository>();
        services.AddSingleton<IPositionRepository, InMemoryPositionRepository>();
        services.AddSingleton<IReportingRelationshipRepository, InMemoryReportingRelationshipRepository>();
        services.AddSingleton<IDelegatedAuthorityRepository, InMemoryDelegatedAuthorityRepository>();
        services.AddSingleton<IAttendanceRecordRepository, InMemoryAttendanceRecordRepository>();
        services.AddSingleton<ILeaveRequestRepository, InMemoryLeaveRequestRepository>();
        services.AddSingleton<IPayrollRunRepository, InMemoryPayrollRunRepository>();
        services.AddSingleton<IPerformanceReviewRepository, InMemoryPerformanceReviewRepository>();
        services.AddSingleton<ICandidateRepository, InMemoryCandidateRepository>();
        services.AddSingleton<ITrainingCourseRepository, InMemoryTrainingCourseRepository>();
        services.AddSingleton<ICourseEnrollmentRepository, InMemoryCourseEnrollmentRepository>();
        services.AddSingleton<ICourseCertificationRepository, InMemoryCourseCertificationRepository>();
        services.AddSingleton<IAnnouncementRepository, InMemoryAnnouncementRepository>();
        services.AddSingleton<IEngagementCampaignRepository, InMemoryEngagementCampaignRepository>();
        services.AddSingleton<IPulseSurveyRepository, InMemoryPulseSurveyRepository>();
        services.AddSingleton<IRecognitionProgramRepository, InMemoryRecognitionProgramRepository>();
        services.AddSingleton<IAnalyticsSnapshotRepository, InMemoryAnalyticsSnapshotRepository>();
        services.AddSingleton<IVacancyRepository, InMemoryVacancyRepository>();
        services.AddSingleton<IInterviewScheduleRepository, InMemoryInterviewScheduleRepository>();
        services.AddSingleton<ISelfServiceAccountRepository, InMemorySelfServiceAccountRepository>();
        services.AddSingleton<ITimesheetRepository, InMemoryTimesheetRepository>();
        services.AddSingleton<ILeaveTypeRepository, InMemoryLeaveTypeRepository>();
        services.AddSingleton<ILeaveBalanceRepository, InMemoryLeaveBalanceRepository>();
    }

    private static void ConfigureIdentityServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        var identityBuilder = services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Lockout.AllowedForNewUsers = true;
        });

        identityBuilder = identityBuilder.AddRoles<IdentityRole<Guid>>();
        identityBuilder.AddEntityFrameworkStores<HrDbContext>();
        identityBuilder.AddSignInManager();
        identityBuilder.AddDefaultTokenProviders();
    }
}
