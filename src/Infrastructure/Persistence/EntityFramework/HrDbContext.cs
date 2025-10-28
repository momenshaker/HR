using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework;

/// <summary>
///     Primary Entity Framework Core database context for the HR platform.
/// </summary>
public sealed class HrDbContext : DbContext
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
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<ReportingRelationship> ReportingRelationships => Set<ReportingRelationship>();
    public DbSet<DelegatedAuthority> DelegatedAuthorities => Set<DelegatedAuthority>();
    public DbSet<SelfServiceAccount> SelfServiceAccounts => Set<SelfServiceAccount>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
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

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
