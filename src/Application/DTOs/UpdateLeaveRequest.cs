using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating a leave request.
/// </summary>
public sealed class UpdateLeaveRequest
{
    [Required]
    [MaxLength(50)]
    public string LeaveType { get; init; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    [MaxLength(500)]
    public string Reason { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    public Guid? ApproverId { get; init; }

    public DateTime? DecisionAtUtc { get; init; }
}
