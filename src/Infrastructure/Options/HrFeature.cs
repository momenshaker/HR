namespace HR.Infrastructure.Options;

/// <summary>
///     Enumerates the high-level feature areas available in the HR platform.
/// </summary>
public enum HrFeature
{
    /// <summary>
    ///     Employee master data, profiles, and lifecycle management.
    /// </summary>
    EmployeeManagement,

    /// <summary>
    ///     Organizational hierarchy, departments, and reporting lines.
    /// </summary>
    OrganizationStructure,

    /// <summary>
    ///     Attendance capture, shift tracking, and time analytics.
    /// </summary>
    AttendanceAndTimeTracking,

    /// <summary>
    ///     Leave policies, balances, and approval workflows.
    /// </summary>
    LeaveManagement,

    /// <summary>
    ///     Payroll calculation, payslips, and statutory deductions.
    /// </summary>
    PayrollManagement,

    /// <summary>
    ///     Performance reviews, goals, and KPI tracking.
    /// </summary>
    PerformanceManagement,

    /// <summary>
    ///     Recruitment pipelines, candidate management, and ATS.
    /// </summary>
    RecruitmentAndAts,

    /// <summary>
    ///     Training catalogues, course tracking, and skill matrices.
    /// </summary>
    TrainingAndDevelopment,

    /// <summary>
    ///     Internal announcements, communications, and feedback.
    /// </summary>
    InternalCommunication,

    /// <summary>
    ///     Analytics dashboards, reporting, and predictive insights.
    /// </summary>
    HrAnalytics
}
