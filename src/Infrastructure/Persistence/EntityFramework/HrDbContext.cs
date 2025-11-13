using HR.Domain.Entities;
using HR.Infrastructure.Persistence.EntityFramework.Seeders;
using HR.Infrastructure.Security.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework;

/// <summary>
///     Primary Entity Framework Core database context for the HR platform.
/// </summary>
public sealed class HrDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HrDbContext" /> class.
    /// </summary>
    /// <param name="options">The configured context options.</param>
    public HrDbContext(DbContextOptions<HrDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<EmployeeDepartment> EmployeeDepartments => Set<EmployeeDepartment>();
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<ReportingRelationship> ReportingRelationships => Set<ReportingRelationship>();
    public DbSet<DelegatedAuthority> DelegatedAuthorities => Set<DelegatedAuthority>();
    public DbSet<SelfServiceAccount> SelfServiceAccounts => Set<SelfServiceAccount>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
    public DbSet<PayrollItem> PayrollItems => Set<PayrollItem>();
    public DbSet<Payslip> Payslips => Set<Payslip>();
    public DbSet<PerformanceReview> PerformanceReviews => Set<PerformanceReview>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<TrainingCourse> TrainingCourses => Set<TrainingCourse>();
    public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();
    public DbSet<CourseCertification> CourseCertifications => Set<CourseCertification>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<EngagementCampaign> EngagementCampaigns => Set<EngagementCampaign>();
    public DbSet<PulseSurvey> PulseSurveys => Set<PulseSurvey>();
    public DbSet<RecognitionProgram> RecognitionPrograms => Set<RecognitionProgram>();
    public DbSet<AnalyticsSnapshot> AnalyticsSnapshots => Set<AnalyticsSnapshot>();
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();
    public DbSet<InterviewSchedule> InterviewSchedules => Set<InterviewSchedule>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionEntitlement> SubscriptionEntitlements => Set<SubscriptionEntitlement>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<UsageCounter> UsageCounters => Set<UsageCounter>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Timesheet> Timesheets => Set<Timesheet>();
    public DbSet<TimesheetEntry> TimesheetEntries => Set<TimesheetEntry>();
    public DbSet<LookupValue> LookupValues => Set<LookupValue>();
    public DbSet<HR.Infrastructure.Security.Identity.UserRefreshToken> UserRefreshTokens => Set<HR.Infrastructure.Security.Identity.UserRefreshToken>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrDbContext).Assembly);

        PlanCatalogSeeder.Seed(modelBuilder);
        OrganizationStructureSeeder.Seed(modelBuilder);
        LookupValueSeeder.Seed(modelBuilder);
    }
}
