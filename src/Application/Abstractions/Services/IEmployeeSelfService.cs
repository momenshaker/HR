using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service exposing employee self-service operations that span multiple HR modules.
/// </summary>
public interface IEmployeeSelfService
{
    /// <summary>
    ///     Retrieves all leave requests created by the specified employee.
    /// </summary>
    Task<IReadOnlyCollection<LeaveRequestDto>> GetLeaveRequestsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Submits a new leave request on behalf of the specified employee.
    /// </summary>
    Task<LeaveRequestDto> SubmitLeaveRequestAsync(
        Guid employeeId,
        CreateLeaveRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves attendance records for the specified employee.
    /// </summary>
    Task<IReadOnlyCollection<AttendanceRecordDto>> GetAttendanceHistoryAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Captures a new clock-in for the specified employee.
    /// </summary>
    Task<AttendanceRecordDto> ClockInAsync(
        Guid employeeId,
        ClockInRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Captures a clock-out for an existing attendance record.
    /// </summary>
    Task<AttendanceRecordDto> ClockOutAsync(
        Guid employeeId,
        Guid attendanceRecordId,
        ClockOutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves salary slips associated with the specified employee.
    /// </summary>
    Task<IReadOnlyCollection<SalarySlipDto>> GetSalarySlipsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves training courses relevant to the specified employee.
    /// </summary>
    Task<IReadOnlyCollection<TrainingCourseDto>> GetTrainingCoursesAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
