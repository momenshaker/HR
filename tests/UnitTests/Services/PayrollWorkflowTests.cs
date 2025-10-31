using HR.Application.Abstractions.Repositories;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class PayrollWorkflowTests
{
    private readonly Mock<IPayrollRunRepository> _runs = new();
    private readonly Mock<IPayrollItemRepository> _items = new();
    private readonly Mock<IPayslipRepository> _payslips = new();
    private readonly Mock<IEmployeeRepository> _employees = new();

    private PayrollService CreateSut()
    {
        return new PayrollService(_runs.Object, _items.Object, _payslips.Object, _employees.Object, new DefaultPayrollCalculator());
    }

    [Fact]
    public async Task CreateRun_Throws_On_Overlap()
    {
        var orgId = Guid.NewGuid();
        var existing = new PayrollRun
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 1, 31),
            CreatedAtUtc = DateTime.UtcNow,
            Status = "Draft"
        };

        _runs.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { existing });

        var sut = CreateSut();

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateRun(orgId, new DateOnly(2025, 1, 15), new DateOnly(2025, 2, 15)));
    }

    [Fact]
    public async Task Calculate_Is_Idempotent()
    {
        var run = new PayrollRun
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 1, 31),
            CreatedAtUtc = DateTime.UtcNow,
            Status = "Draft"
        };

        _runs.Setup(r => r.GetByIdAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(run);
        _runs.Setup(r => r.UpdateAsync(It.IsAny<PayrollRun>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PayrollRun pr, CancellationToken _) => pr);
        _items.Setup(i => i.GetByRunAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<PayrollItem>());
        _items.Setup(i => i.AddRangeAsync(It.IsAny<IEnumerable<PayrollItem>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<PayrollItem>, CancellationToken>((col, _) => _items.Setup(ii => ii.GetByRunAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(col.ToArray()))
            .Returns(Task.CompletedTask);
        _employees.Setup(e => e.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { new Employee { Id = Guid.NewGuid() } });

        var sut = CreateSut();

        var first = await sut.Calculate(run.Id);
        var second = await sut.Calculate(run.Id);

        Assert.Equal("Calculated", first.Status);
        Assert.Equal(first.TotalGrossPay, second.TotalGrossPay);
        _items.Verify(i => i.AddRangeAsync(It.IsAny<IEnumerable<PayrollItem>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Approve_Then_Paid_Enforces_Transitions()
    {
        var run = new PayrollRun
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 1, 31),
            CreatedAtUtc = DateTime.UtcNow,
            Status = "Calculated"
        };

        _runs.Setup(r => r.GetByIdAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(run);
        _runs.Setup(r => r.UpdateAsync(It.IsAny<PayrollRun>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PayrollRun pr, CancellationToken _) => pr);

        var sut = CreateSut();

        var approved = await sut.Approve(run.Id);
        Assert.Equal("Approved", approved.Status);

        var paid = await sut.MarkPaid(run.Id);
        Assert.Equal("Paid", paid.Status);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Approve(run.Id));
    }

    [Fact]
    public async Task GeneratePayslips_Is_Idempotent()
    {
        var run = new PayrollRun
        {
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            PeriodStart = new DateOnly(2025, 1, 1),
            PeriodEnd = new DateOnly(2025, 1, 31),
            CreatedAtUtc = DateTime.UtcNow,
            Status = "Calculated"
        };

        _runs.Setup(r => r.GetByIdAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(run);
        _items.Setup(i => i.GetByRunAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new[]
        {
            new PayrollItem { Id = Guid.NewGuid(), RunId = run.Id, EmployeeId = Guid.NewGuid() },
            new PayrollItem { Id = Guid.NewGuid(), RunId = run.Id, EmployeeId = Guid.NewGuid() }
        });

        var stored = new List<Payslip>();
        _payslips.Setup(p => p.GetByRunAsync(run.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => stored.ToArray());
        _payslips.Setup(p => p.AddRangeAsync(It.IsAny<IEnumerable<Payslip>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<Payslip>, CancellationToken>((ps, _) => stored.AddRange(ps))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();
        var first = await sut.GeneratePayslips(run.Id);
        var second = await sut.GeneratePayslips(run.Id);

        Assert.Equal(first.Count, second.Count);
        Assert.Equal(2, first.Count);
    }
}

