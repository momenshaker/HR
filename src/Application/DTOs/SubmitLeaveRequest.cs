using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

public sealed class SubmitLeaveRequest
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
    public string? Reason { get; init; }

    [MaxLength(260)]
    public string? AttachmentPath { get; init; }
}

