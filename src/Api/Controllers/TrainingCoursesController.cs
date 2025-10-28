using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for training and development operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[FeatureRequirement(HrFeature.TrainingAndDevelopment)]
public sealed class TrainingCoursesController(ITrainingService trainingService) : ControllerBase
{
    private readonly ITrainingService _trainingService = trainingService;

    /// <summary>
    ///     Retrieves all training courses.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TrainingCourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var courses = await _trainingService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(courses);
    }

    /// <summary>
    ///     Retrieves a training course by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TrainingCourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var course = await _trainingService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return course is null ? NotFound() : Ok(course);
    }

    /// <summary>
    ///     Creates a new training course.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TrainingCourseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateTrainingCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdCourse = await _trainingService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdCourse.Id }, createdCourse);
    }

    /// <summary>
    ///     Updates an existing training course.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TrainingCourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateTrainingCourseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updatedCourse = await _trainingService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updatedCourse is null ? NotFound() : Ok(updatedCourse);
    }

    /// <summary>
    ///     Deletes a training course.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _trainingService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
