using System;
using System.ComponentModel.DataAnnotations;
using HR.Domain.Entities;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload for updating enrollment progress and status.
/// </summary>
public sealed class UpdateCourseEnrollmentProgressRequest : IValidatableRequest
{
    [Range(0, 100)]
    public decimal CompletionPercentage { get; init; }

    public CourseEnrollmentStatus Status { get; init; } = CourseEnrollmentStatus.InProgress;

    public DateOnly? CompletedOn { get; init; }
}