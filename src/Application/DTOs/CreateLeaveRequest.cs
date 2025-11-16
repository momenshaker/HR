using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a leave request.
/// </summary>
public sealed class CreateLeaveRequest : IValidatableRequest
{
    [Required]
    public Guid EmployeeId { get; init; }

    [Required]
    public Guid LeaveTypeId { get; init; }

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    [MaxLength(500)]
    public string Reason { get; init; } = string.Empty;

    [MaxLength(260)]
    public string? AttachmentPath { get; init; }
}