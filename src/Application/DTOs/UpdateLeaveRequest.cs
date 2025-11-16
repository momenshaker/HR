using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating a leave request.
/// </summary>
public sealed class UpdateLeaveRequest : IValidatableRequest
{
    [Required]
    public Guid LeaveTypeId { get; init; }

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    [MaxLength(500)]
    public string Reason { get; init; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; init; } = string.Empty;

    public Guid? ApproverId { get; init; }

    public DateTime? ApprovedAtUtc { get; init; }

    public DateTime? RejectedAtUtc { get; init; }

    public DateTime? CancelledAtUtc { get; init; }

    [MaxLength(260)]
    public string? AttachmentPath { get; init; }
}