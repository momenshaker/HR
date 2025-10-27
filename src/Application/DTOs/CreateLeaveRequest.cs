using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a leave request.
/// </summary>
public sealed class CreateLeaveRequest
{
    [Required]
    public Guid EmployeeId { get; init; }

    [Required]
    [MaxLength(50)]
    public string LeaveType { get; init; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    [MaxLength(500)]
    public string Reason { get; init; } = string.Empty;
}
