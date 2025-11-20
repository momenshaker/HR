using HR.Api.Authorization;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for training and development operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[RolePermission(
    "training",
    readRoles: new[] { "Admin", "HR", "Manager", "Employee" },
    writeRoles: new[] { "Admin", "HR", "Manager" })]
[AuditResource("TrainingCourse")]
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
    ///     Retrieves training courses aligned to the specified competency code.
    /// </summary>
    [HttpGet("competency/{competencyCode}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TrainingCourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByCompetencyAsync(string competencyCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(competencyCode))
        {
            ModelState.AddModelError(nameof(competencyCode), "Competency code is required.");
            return ValidationProblem(ModelState);
        }

        var courses = await _trainingService.GetByCompetencyAsync(competencyCode, cancellationToken).ConfigureAwait(false);
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

    /// <summary>
    ///     Enrolls an employee in a training course.
    /// </summary>
    [HttpPost("{courseId:guid}/enrollments")]
    [ProducesResponseType(typeof(CourseEnrollmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EnrollAsync(Guid courseId, [FromBody] CreateCourseEnrollmentRequest request, CancellationToken cancellationToken)
    {

        if (request.CourseId != courseId)
        {
            ModelState.AddModelError(nameof(request.CourseId), "Course identifier mismatch between route and payload.");
            return ValidationProblem(ModelState);
        }

        var enrollment = await _trainingService.EnrollEmployeeAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetCourseEnrollmentsAsync), new { courseId }, enrollment);
    }

    /// <summary>
    ///     Retrieves all enrollments for a training course.
    /// </summary>
    [HttpGet("{courseId:guid}/enrollments")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CourseEnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseEnrollmentsAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var enrollments = await _trainingService.GetCourseEnrollmentsAsync(courseId, cancellationToken).ConfigureAwait(false);
        return Ok(enrollments);
    }

    /// <summary>
    ///     Retrieves enrollments for a specific employee.
    /// </summary>
    [HttpGet("enrollments/employee/{employeeId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CourseEnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeEnrollmentsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var enrollments = await _trainingService.GetEmployeeEnrollmentsAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(enrollments);
    }

    /// <summary>
    ///     Updates progress for an enrollment.
    /// </summary>
    [HttpPatch("enrollments/{enrollmentId:guid}/progress")]
    [ProducesResponseType(typeof(CourseEnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentProgressAsync(Guid enrollmentId, [FromBody] UpdateCourseEnrollmentProgressRequest request, CancellationToken cancellationToken)
    {

        var updated = await _trainingService.UpdateEnrollmentProgressAsync(enrollmentId, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Withdraws an enrollment from a course.
    /// </summary>
    [HttpPost("enrollments/{enrollmentId:guid}/withdraw")]
    [ProducesResponseType(typeof(CourseEnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> WithdrawEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        var updated = await _trainingService.WithdrawEnrollmentAsync(enrollmentId, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Provides progress analytics for a course.
    /// </summary>
    [HttpGet("{courseId:guid}/analytics")]
    [ProducesResponseType(typeof(CourseProgressAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseAnalyticsAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var analytics = await _trainingService.GetCourseProgressAnalyticsAsync(courseId, cancellationToken).ConfigureAwait(false);
        return Ok(analytics);
    }

    /// <summary>
    ///     Issues a certification for a completed enrollment.
    /// </summary>
    [HttpPost("{courseId:guid}/certifications")]
    [ProducesResponseType(typeof(CourseCertificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IssueCertificationAsync(Guid courseId, [FromBody] IssueCourseCertificationRequest request, CancellationToken cancellationToken)
    {

        if (request.CourseId != courseId)
        {
            ModelState.AddModelError(nameof(request.CourseId), "Course identifier mismatch between route and payload.");
            return ValidationProblem(ModelState);
        }

        var certification = await _trainingService.IssueCertificationAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetCourseCertificationsAsync), new { courseId }, certification);
    }

    /// <summary>
    ///     Revokes a certification with governance notes.
    /// </summary>
    [HttpPost("certifications/{certificationId:guid}/revoke")]
    [ProducesResponseType(typeof(CourseCertificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeCertificationAsync(Guid certificationId, [FromBody] RevokeCourseCertificationRequest request, CancellationToken cancellationToken)
    {

        var revoked = await _trainingService.RevokeCertificationAsync(certificationId, request, cancellationToken).ConfigureAwait(false);
        return revoked is null ? NotFound() : Ok(revoked);
    }

    /// <summary>
    ///     Retrieves certifications issued for a course.
    /// </summary>
    [HttpGet("{courseId:guid}/certifications")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CourseCertificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseCertificationsAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var certifications = await _trainingService.GetCourseCertificationsAsync(courseId, cancellationToken).ConfigureAwait(false);
        return Ok(certifications);
    }

    /// <summary>
    ///     Retrieves certifications earned by an employee.
    /// </summary>
    [HttpGet("certifications/employee/{employeeId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CourseCertificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeCertificationsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var certifications = await _trainingService.GetEmployeeCertificationsAsync(employeeId, cancellationToken).ConfigureAwait(false);
        return Ok(certifications);
    }
}
