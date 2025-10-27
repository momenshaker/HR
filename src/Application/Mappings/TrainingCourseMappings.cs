using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="TrainingCourse" /> entities.
/// </summary>
public static class TrainingCourseMappings
{
    public static TrainingCourseDto ToDto(this TrainingCourse course)
    {
        ArgumentNullException.ThrowIfNull(course);

        return new TrainingCourseDto(
            course.Id,
            course.Title,
            course.Category,
            course.Description,
            course.Instructor,
            course.StartDate,
            course.EndDate,
            course.Capacity,
            course.DeliveryMode);
    }

    public static TrainingCourse ToEntity(this CreateTrainingCourseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new TrainingCourse
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Category = request.Category.Trim(),
            Description = request.Description.Trim(),
            Instructor = request.Instructor.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Capacity = request.Capacity,
            DeliveryMode = request.DeliveryMode.Trim()
        };
    }

    public static TrainingCourse ApplyUpdates(this UpdateTrainingCourseRequest request, TrainingCourse existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new TrainingCourse
        {
            Id = existing.Id,
            Title = request.Title.Trim(),
            Category = request.Category.Trim(),
            Description = request.Description.Trim(),
            Instructor = request.Instructor.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Capacity = request.Capacity,
            DeliveryMode = request.DeliveryMode.Trim()
        };
    }
}
