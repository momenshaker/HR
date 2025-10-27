namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a training course.
/// </summary>
public sealed record TrainingCourseDto(
    Guid Id,
    string Title,
    string Category,
    string Description,
    string Instructor,
    DateOnly StartDate,
    DateOnly EndDate,
    int Capacity,
    string DeliveryMode);
