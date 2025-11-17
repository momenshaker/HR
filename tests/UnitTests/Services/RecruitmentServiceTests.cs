using System.Collections.Generic;
using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class RecruitmentServiceTests
{
    private readonly Mock<ICandidateRepository> _candidateRepositoryMock = new();
    private readonly Mock<IInterviewScheduleRepository> _interviewRepositoryMock = new();
    private readonly Mock<IVacancyRepository> _vacancyRepositoryMock = new();
    private readonly RecruitmentService _sut;

    public RecruitmentServiceTests()
    {
        _sut = new RecruitmentService(
            _candidateRepositoryMock.Object,
            _vacancyRepositoryMock.Object,
            _interviewRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_PersistsCandidate()
    {
        // Arrange
        var request = new CreateCandidateRequest
        {
            FullName = " Jane Doe ",
            Email = "jane.doe@example.com",
            AppliedRole = "Engineer",
            Stage = "Applied"
        };

        Candidate? persisted = null;

        _candidateRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Candidate>(), It.IsAny<CancellationToken>()))
            .Callback<Candidate, CancellationToken>((candidate, _) => persisted = candidate)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.Equal("Jane Doe", persisted!.FullName);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdateCandidateRequest
        {
            FullName = "Jane Doe",
            Email = "jane.doe@example.com",
            AppliedRole = "Engineer"
        };

        _candidateRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Candidate?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _candidateRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Candidate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AdvanceCandidateAsync_WithInterview_SchedulesAndUpdates()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var vacancyId = Guid.NewGuid();
        var interviewTime = DateTime.UtcNow.AddDays(1);
        var existingCandidate = new Candidate
        {
            Id = candidateId,
            FullName = "Jane Doe",
            Email = "jane.doe@example.com",
            AppliedRole = "Engineer",
            Stage = "Applied",
            Source = "Referral",
            AppliedAtUtc = DateTime.UtcNow.AddDays(-7),
            ResumeUrl = "https://example.com/resume.pdf",
            Notes = "Initial application"
        };

        _candidateRepositoryMock
            .Setup(repo => repo.GetByIdAsync(candidateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCandidate);

        Candidate? persistedCandidate = null;
        _candidateRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Candidate>(), It.IsAny<CancellationToken>()))
            .Callback<Candidate, CancellationToken>((candidate, _) => persistedCandidate = candidate)
            .ReturnsAsync(() => persistedCandidate);

        InterviewSchedule? scheduledInterview = null;
        _interviewRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<InterviewSchedule>(), It.IsAny<CancellationToken>()))
            .Callback<InterviewSchedule, CancellationToken>((schedule, _) => scheduledInterview = schedule)
            .ReturnsAsync(() => scheduledInterview!);

        var request = new AdvanceCandidateRequest
        {
            TargetStage = "Interview",
            Notes = "Ready for interview",
            Interview = new ScheduleInterviewRequest
            {
                CandidateId = Guid.Empty,
                VacancyId = vacancyId,
                Stage = string.Empty,
                ScheduledAtUtc = interviewTime,
                DurationMinutes = 60,
                Mode = "Remote",
                Location = "Teams",
                MeetingLink = "https://meeting",
                Interviewers = new[] { "Hiring Manager" },
                Notes = "Technical screening"
            }
        };

        // Act
        var result = await _sut.AdvanceCandidateAsync(candidateId, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(persistedCandidate);
        Assert.Equal("Interview", persistedCandidate!.Stage);
        Assert.Equal(interviewTime, persistedCandidate.NextInterviewAtUtc);
        Assert.Contains("Ready for interview", persistedCandidate.Notes);

        Assert.NotNull(scheduledInterview);
        Assert.Equal(candidateId, scheduledInterview!.CandidateId);
        Assert.Equal(vacancyId, scheduledInterview.VacancyId);
        Assert.Equal("Interview", scheduledInterview.Stage);
    }

    [Fact]
    public async Task PublishVacancyAsync_PublishesWithNormalizedData()
    {
        // Arrange
        var request = new CreateVacancyRequest
        {
            RequisitionId = Guid.NewGuid(),
            PublicTitle = " Senior Engineer ",
            Department = " Engineering ",
            Location = "Remote",
            EmploymentType = "Full-time",
            PublicDescription = "Lead strategic initiatives",
            Responsibilities = new[] { " Build systems ", "build systems" },
            Requirements = new[] { "C#", ".NET" },
            PipelineStages = new List<string> { "Applied", "Interview" },
            HiringTeam = new List<string> { "Alice", "alice" },
            ApplicationUrl = "https://example.com/jobs/123"
        };

        Vacancy? persistedVacancy = null;
        _vacancyRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Vacancy>(), It.IsAny<CancellationToken>()))
            .Callback<Vacancy, CancellationToken>((vacancy, _) => persistedVacancy = vacancy)
            .ReturnsAsync(() => persistedVacancy!);

        // Act
        var result = await _sut.PublishVacancyAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persistedVacancy);
        Assert.Equal("Senior Engineer", persistedVacancy!.PublicTitle);
        Assert.Equal("Published", persistedVacancy.Status);
        Assert.Single(persistedVacancy.Responsibilities);
        Assert.Equal(result.Id, persistedVacancy.Id);
    }

    [Fact]
    public async Task GetInsightsAsync_ComposesInsightDto()
    {
        // Arrange
        var vacancyId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var interviewId = Guid.NewGuid();
        var futureTime = DateTime.UtcNow.AddHours(4);

        _vacancyRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Vacancy>
            {
            new()
            {
                Id = vacancyId,
                PublicTitle = "Engineer",
                Department = "Engineering",
                Location = "Remote",
                EmploymentType = "Full-time",
                PublicDescription = "Build",
                Responsibilities = new List<string> { "Code" },
                Requirements = new List<string> { "C#" },
                PipelineStages = new List<string> { "Applied", "Interview" },
                HiringTeam = new List<string> { "Alice" },
                PublishedAtUtc = DateTime.UtcNow.AddDays(-10),
                Status = "Published",
                ApplicationUrl = "https://example.com"
            },
            new()
            {
                Id = Guid.NewGuid(),
                PublicTitle = "Designer",
                Department = "Design",
                Location = "Hybrid",
                EmploymentType = "Contract",
                PublicDescription = "Design",
                Responsibilities = new List<string>(),
                Requirements = new List<string>(),
                PipelineStages = new List<string> { "Applied" },
                HiringTeam = new List<string>(),
                PublishedAtUtc = DateTime.UtcNow.AddDays(-20),
                ClosedAtUtc = DateTime.UtcNow.AddDays(-1),
                Status = "Closed",
                ApplicationUrl = string.Empty
            }
            });

        _candidateRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Candidate>
            {
                new()
                {
                    Id = candidateId,
                    FullName = "Jane Doe",
                    Email = "jane@example.com",
                    AppliedRole = "Engineer",
                    Stage = "Interview",
                    Source = "Referral",
                    AppliedAtUtc = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Smith",
                    Email = "john@example.com",
                    AppliedRole = "Engineer",
                    Stage = "Applied",
                    Source = "Career page",
                    AppliedAtUtc = DateTime.UtcNow.AddDays(-2)
                }
            });

        _interviewRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InterviewSchedule>
            {
                new()
                {
                    Id = interviewId,
                    CandidateId = candidateId,
                    VacancyId = vacancyId,
                    Stage = "Interview",
                    ScheduledAtUtc = futureTime,
                    Duration = TimeSpan.FromMinutes(60),
                    Mode = "Remote",
                    Interviewers = new List<string> { "Alice" },
                    Status = "Scheduled"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CandidateId = candidateId,
                    VacancyId = vacancyId,
                    Stage = "Interview",
                    ScheduledAtUtc = DateTime.UtcNow.AddHours(-2),
                    Duration = TimeSpan.FromMinutes(30),
                    Mode = "Remote",
                    Interviewers = new List<string> { "Bob" },
                    Status = "Cancelled"
                }
            });

        // Act
        var result = await _sut.GetInsightsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalVacancies);
        Assert.Equal(1, result.OpenVacancies);
        Assert.Equal(2, result.ActiveCandidates);
        Assert.Contains(result.PipelineStageSummaries, summary => summary.Stage == "Interview" && summary.CandidateCount == 1);
        Assert.Single(result.UpcomingInterviews);
        Assert.Single(result.HiringTeamCollaborators);
    }
}
