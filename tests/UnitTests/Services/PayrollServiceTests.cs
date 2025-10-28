using System.Linq;
using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class PayrollServiceTests
{
    private readonly Mock<IPayrollRunRepository> _repositoryMock = new();
    private readonly PayrollService _sut;

    public PayrollServiceTests()
    {
        _sut = new PayrollService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_SetsProcessedTimestamp()
    {
        // Arrange
        var request = new CreatePayrollRunRequest
        {
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 1, 31),
            Status = "Completed",
            TotalGrossPay = 1000,
            TotalNetPay = 800
        };

        PayrollRun? persisted = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<PayrollRun>(), It.IsAny<CancellationToken>()))
            .Callback<PayrollRun, CancellationToken>((run, _) => persisted = run)
            .ReturnsAsync(() => persisted!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persisted);
        Assert.True(persisted!.ProcessedAtUtc <= DateTime.UtcNow);
        Assert.Equal(result.Id, persisted.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNull()
    {
        // Arrange
        var request = new UpdatePayrollRunRequest
        {
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 1, 31),
            Status = "Completed",
            TotalGrossPay = 1000,
            TotalNetPay = 800,
            Notes = string.Empty,
            ProcessedAtUtc = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PayrollRun?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<PayrollRun>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetSalarySlipsAsync_ProjectsRunsToSlips()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var payrollRuns = new[]
        {
            new PayrollRun
            {
                Id = Guid.NewGuid(),
                PeriodStart = new DateOnly(2025, 1, 1),
                PeriodEnd = new DateOnly(2025, 1, 31),
                ProcessedAtUtc = DateTime.UtcNow.AddDays(-10),
                Status = "Completed",
                TotalGrossPay = 10_000m,
                TotalNetPay = 8_000m,
                Notes = "Run A"
            },
            new PayrollRun
            {
                Id = Guid.NewGuid(),
                PeriodStart = new DateOnly(2024, 12, 1),
                PeriodEnd = new DateOnly(2024, 12, 31),
                ProcessedAtUtc = DateTime.UtcNow.AddDays(-40),
                Status = "Completed",
                TotalGrossPay = 9_500m,
                TotalNetPay = 7_600m,
                Notes = "Run B"
            }
        };

        _repositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(payrollRuns);

        // Act
        var result = await _sut.GetSalarySlipsAsync(employeeId, CancellationToken.None);

        // Assert
        Assert.Equal(payrollRuns.Length, result.Count);
        Assert.All(result, slip => Assert.Equal(employeeId, slip.EmployeeId));
        Assert.Equal(payrollRuns[0].Id, result.First().PayrollRunId);
    }
}
