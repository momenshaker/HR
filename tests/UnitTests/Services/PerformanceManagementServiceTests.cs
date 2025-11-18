using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class PerformanceManagementServiceTests
{
    private readonly Mock<IPerformanceRepository> _repositoryMock = new();
    private readonly PerformanceManagementService _sut;

    public PerformanceManagementServiceTests()
    {
        _sut = new PerformanceManagementService(_repositoryMock.Object);
    }

    [Fact]
    public async Task ActivateCycle_GeneratesEvaluations()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var scaleId = Guid.NewGuid();
        var cycleId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        var template = new EvaluationTemplate
        {
            Id = templateId,
            Name = "Staff",
            RatingScaleId = scaleId,
            IsActive = true,
            Sections = new[]
            {
                new TemplateSectionDefinition
                {
                    Id = Guid.NewGuid(),
                    TemplateId = templateId,
                    Name = "Core",
                    Weight = 60,
                    Items = new[]
                    {
                        new TemplateItemDefinition { Id = Guid.NewGuid(), SectionDefinitionId = Guid.Empty, Name = "Quality", DefaultWeight = 100 }
                    }
                }
            }
        };

        var cycle = new PerformanceCycle
        {
            Id = cycleId,
            Name = "2025 Annual",
            Description = "Annual review",
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 12, 31),
            SelfEvaluationStart = new DateOnly(2025, 1, 1),
            SelfEvaluationEnd = new DateOnly(2025, 2, 1),
            ManagerEvaluationStart = new DateOnly(2025, 2, 1),
            ManagerEvaluationEnd = new DateOnly(2025, 3, 1),
            TemplateId = templateId,
            RatingScaleId = scaleId,
            IncludedEmployees = new[] { new PerformanceCycleAssignment { EmployeeId = employeeId, ManagerId = Guid.NewGuid(), Department = "Engineering" } },
            CreatedBy = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(repo => repo.GetCycleAsync(cycleId, It.IsAny<CancellationToken>())).ReturnsAsync(cycle);
        _repositoryMock.Setup(repo => repo.GetTemplateAsync(templateId, It.IsAny<CancellationToken>())).ReturnsAsync(template);
        _repositoryMock.Setup(repo => repo.GetRatingScaleAsync(scaleId, It.IsAny<CancellationToken>())).ReturnsAsync(new RatingScale { Id = scaleId });
        _repositoryMock.Setup(repo => repo.AddEvaluationAsync(It.IsAny<Evaluation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evaluation evaluation, CancellationToken _) => evaluation);
        _repositoryMock.Setup(repo => repo.UpdateCycleAsync(It.IsAny<PerformanceCycle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PerformanceCycle updated, CancellationToken _) => updated);
        _repositoryMock.Setup(repo => repo.GetEvaluationsByCycleAsync(cycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Evaluation>());

        // Act
        var result = await _sut.ActivateCycleAsync(cycleId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _repositoryMock.Verify(repo => repo.AddEvaluationAsync(It.IsAny<Evaluation>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        Assert.Equal(PerformanceCycleStatus.Active, result!.Status);
    }

    [Fact]
    public async Task SubmitManagerEvaluation_UpdatesStatus()
    {
        // Arrange
        var evaluationId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var evaluation = new Evaluation
        {
            Id = evaluationId,
            EmployeeId = Guid.NewGuid(),
            CycleId = Guid.NewGuid(),
            TemplateId = Guid.NewGuid(),
            Status = EvaluationStatus.SelfCompleted,
            Sections = new[]
            {
                new EvaluationSection
                {
                    Id = sectionId,
                    EvaluationId = evaluationId,
                    TemplateSectionDefinitionId = Guid.NewGuid(),
                    Name = "Core",
                    Weight = 100,
                    Items = new[]
                    {
                        new EvaluationItem
                        {
                            Id = itemId,
                            EvaluationSectionId = sectionId,
                            TemplateItemDefinitionId = Guid.NewGuid(),
                            Name = "Quality",
                            Weight = 100,
                            SelfScore = 3
                        }
                    }
                }
            },
            Participants = Array.Empty<EvaluationParticipant>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(repo => repo.GetEvaluationAsync(evaluationId, It.IsAny<CancellationToken>())).ReturnsAsync(evaluation);
        _repositoryMock
            .Setup(repo => repo.UpdateEvaluationAsync(It.IsAny<Evaluation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evaluation updated, CancellationToken _) => updated);

        var request = new SubmitEvaluationRequest
        {
          Comments = "Manager feedback",
          Sections = new[]
          {
              new SubmittedSectionRequest
              {
                  SectionId = sectionId,
                  Items = new[] { new SubmittedItemRequest { ItemId = itemId, Score = 4, Comment = "Great" } }
              }
          }
        };

        // Act
        var result = await _sut.SubmitManagerEvaluationAsync(evaluationId, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(EvaluationStatus.ManagerCompleted, result!.Status);
        Assert.Equal(4, result.Sections.First().Items.First().ManagerScore);
    }
}
