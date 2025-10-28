using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class TrainingServiceTests
{
    private readonly Mock<ITrainingCourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseEnrollmentRepository> _enrollmentRepositoryMock = new();
    private readonly Mock<ICourseCertificationRepository> _certificationRepositoryMock = new();
    private readonly TrainingService _sut;

    public TrainingServiceTests()
    {
        _sut = new TrainingService(
            _courseRepositoryMock.Object,
            _enrollmentRepositoryMock.Object,
            _certificationRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsCourse()
    {
        // Arrange
        var request = new CreateTrainingCourseRequest
        {
            Title = " Leadership 101 ",
            StartDate = new DateOnly(2025, 2, 1),
            EndDate = new DateOnly(2025, 2, 15),
            Capacity = 20
        };

        TrainingCourse? persisted = null;

        _courseRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<TrainingCourse>(), It.IsAny<CancellationToken>()))
            .Callback<TrainingCourse, CancellationToken>((course, _) => persisted = course)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Leadership 101", persisted!.Title);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateTrainingCourseRequest
        {
            Title = "Leadership 101",
            StartDate = new DateOnly(2025, 2, 1),
            EndDate = new DateOnly(2025, 2, 15),
            Capacity = 20
        };

        _courseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingCourse?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _courseRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<TrainingCourse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task EnrollEmployeeAsync_PersistsEnrollment()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var course = new TrainingCourse { Id = courseId, OffersCertification = true };

        _courseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _enrollmentRepositoryMock
            .Setup(repo => repo.GetByCourseAndEmployeeAsync(courseId, employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseEnrollment?)null);

        CourseEnrollment? persisted = null;

        _enrollmentRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<CourseEnrollment>(), It.IsAny<CancellationToken>()))
            .Callback<CourseEnrollment, CancellationToken>((enrollment, _) => persisted = enrollment)
            .ReturnsAsync(() => persisted!);

        var request = new CreateCourseEnrollmentRequest
        {
            CourseId = courseId,
            EmployeeId = employeeId
        };

        // Act
        var result = await _sut.EnrollEmployeeAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal(courseId, persisted!.CourseId);
        Assert.Equal(employeeId, result.EmployeeId);
    }

    [Fact]
    public async Task GetCourseProgressAnalyticsAsync_ComputesAverages()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var enrollments = new List<CourseEnrollment>
        {
            new() { Id = Guid.NewGuid(), CourseId = courseId, EmployeeId = Guid.NewGuid(), CompletionPercentage = 100m, Status = CourseEnrollmentStatus.Completed },
            new() { Id = Guid.NewGuid(), CourseId = courseId, EmployeeId = Guid.NewGuid(), CompletionPercentage = 40m, Status = CourseEnrollmentStatus.InProgress }
        };

        _enrollmentRepositoryMock
            .Setup(repo => repo.GetByCourseAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollments);

        // Act
        var analytics = await _sut.GetCourseProgressAnalyticsAsync(courseId, CancellationToken.None);

        // Assert
        Assert.Equal(2, analytics.TotalEnrollments);
        Assert.Equal(1, analytics.CompletedEnrollments);
        Assert.Equal(70m, analytics.AverageCompletionPercentage);
    }
}
