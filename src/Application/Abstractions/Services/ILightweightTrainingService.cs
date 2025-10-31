using HR.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.Application.Abstractions.Services;

public interface ILightweightTrainingService
{
    Task<IReadOnlyCollection<LiteCourseDto>> GetCoursesAsync(Guid organizationId, CancellationToken cancellationToken = default);

    Task<LiteCourseDto> CreateCourseAsync(CreateLiteCourseRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LiteCourseSessionDto>> GetCourseSessionsAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<LiteCourseSessionDto> CreateCourseSessionAsync(CreateLiteCourseSessionRequest request, CancellationToken cancellationToken = default);

    Task<LiteEnrollmentDto> EnrollAsync(Guid sessionId, Guid employeeId, CancellationToken cancellationToken = default);

    Task<LiteEnrollmentDto> CompleteAsync(Guid sessionId, Guid employeeId, CancellationToken cancellationToken = default);

    Task<LiteEnrollmentDto> CancelAsync(Guid sessionId, Guid employeeId, CancellationToken cancellationToken = default);

    // Mandatory training report: map EmployeeId -> missing CourseIds
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Guid>>> GetMandatoryCompletionGapsAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

