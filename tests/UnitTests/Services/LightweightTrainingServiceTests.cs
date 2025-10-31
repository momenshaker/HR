using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Infrastructure.Persistence.Repositories;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class LightweightTrainingServiceTests
{
    private readonly ICourseRepository _courseRepo = new InMemoryCourseRepository();
    private readonly ICourseSessionRepository _sessionRepo = new InMemoryCourseSessionRepository();
    private readonly ICourseSessionEnrollmentRepository _enrollmentRepo = new InMemoryCourseSessionEnrollmentRepository();

    private LightweightTrainingService CreateSut() => new(_courseRepo, _sessionRepo, _enrollmentRepo);

    [Fact]
    public async Task Capacity_Is_Enforced_On_Enroll()
    {
        var sut = CreateSut();
        var orgId = Guid.NewGuid();

        var course = await sut.CreateCourseAsync(new CreateLiteCourseRequest
        {
            OrganizationId = orgId,
            Code = "SAFE-101",
            Title = "Safety",
            DurationHours = 2.5m,
            IsMandatory = true
        }, CancellationToken.None);

        var session = await sut.CreateCourseSessionAsync(new CreateLiteCourseSessionRequest
        {
            CourseId = course.Id,
            StartUtc = DateTime.UtcNow.AddDays(1),
            EndUtc = DateTime.UtcNow.AddDays(1).AddHours(2),
            Location = "Room A",
            Capacity = 1
        }, CancellationToken.None);

        var emp1 = Guid.NewGuid();
        var emp2 = Guid.NewGuid();

        await sut.EnrollAsync(session.Id, emp1, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.EnrollAsync(session.Id, emp2, CancellationToken.None));
    }

    [Fact]
    public async Task Status_Transitions_Work_As_Expected()
    {
        var sut = CreateSut();
        var orgId = Guid.NewGuid();
        var emp = Guid.NewGuid();

        var course = await sut.CreateCourseAsync(new CreateLiteCourseRequest
        {
            OrganizationId = orgId,
            Code = "CODE-1",
            Title = "Course",
            DurationHours = 1m
        }, CancellationToken.None);

        var session = await sut.CreateCourseSessionAsync(new CreateLiteCourseSessionRequest
        {
            CourseId = course.Id,
            StartUtc = DateTime.UtcNow.AddDays(1),
            EndUtc = DateTime.UtcNow.AddDays(1).AddHours(1),
            MeetingUrl = "https://meet/1"
        }, CancellationToken.None);

        var e1 = await sut.EnrollAsync(session.Id, emp, CancellationToken.None);
        Assert.Equal(LiteEnrollmentStatus.Enrolled, e1.Status);

        var completed = await sut.CompleteAsync(session.Id, emp, CancellationToken.None);
        Assert.Equal(LiteEnrollmentStatus.Completed, completed.Status);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CancelAsync(session.Id, emp, CancellationToken.None));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CompleteAsync(session.Id, emp, CancellationToken.None));
    }

    [Fact]
    public async Task Mandatory_Completion_Query_Identifies_Gaps()
    {
        var sut = CreateSut();
        var orgId = Guid.NewGuid();
        var emp = Guid.NewGuid();

        // Two mandatory courses
        var c1 = await sut.CreateCourseAsync(new CreateLiteCourseRequest
        {
            OrganizationId = orgId,
            Code = "M-1",
            Title = "M1",
            DurationHours = 1,
            IsMandatory = true
        }, CancellationToken.None);

        var c2 = await sut.CreateCourseAsync(new CreateLiteCourseRequest
        {
            OrganizationId = orgId,
            Code = "M-2",
            Title = "M2",
            DurationHours = 1,
            IsMandatory = true
        }, CancellationToken.None);

        // Sessions
        var s1 = await sut.CreateCourseSessionAsync(new CreateLiteCourseSessionRequest
        {
            CourseId = c1.Id,
            StartUtc = DateTime.UtcNow.AddDays(1),
            EndUtc = DateTime.UtcNow.AddDays(1).AddHours(1),
            Location = "L1"
        }, CancellationToken.None);

        var s2 = await sut.CreateCourseSessionAsync(new CreateLiteCourseSessionRequest
        {
            CourseId = c2.Id,
            StartUtc = DateTime.UtcNow.AddDays(2),
            EndUtc = DateTime.UtcNow.AddDays(2).AddHours(1),
            Location = "L2"
        }, CancellationToken.None);

        // Employee completes only c1
        await sut.EnrollAsync(s1.Id, emp, CancellationToken.None);
        await sut.CompleteAsync(s1.Id, emp, CancellationToken.None);

        var gaps = await sut.GetMandatoryCompletionGapsAsync(orgId, CancellationToken.None);

        Assert.True(gaps.ContainsKey(emp));
        var missing = gaps[emp];
        Assert.Contains(c2.Id, missing);
        Assert.DoesNotContain(c1.Id, missing);
    }
}

