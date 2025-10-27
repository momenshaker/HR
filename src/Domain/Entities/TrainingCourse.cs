namespace HR.Domain.Entities;

/// <summary>
///     Represents a training course that employees can enroll in.
/// </summary>
public sealed class TrainingCourse
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Instructor { get; init; } = string.Empty;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public int Capacity { get; init; }

    public string DeliveryMode { get; init; } = string.Empty;
}
