using HR.Application.Abstractions.Repositories;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class PayrollServiceTests
{
    [Fact]
    public async Task CreateRun_Sets_Draft_Status_And_Timestamps()
    {
        var runs = new Mock<IPayrollRunRepository>();
        var items = new Mock<IPayrollItemRepository>();
        var payslips = new Mock<IPayslipRepository>();
        var employees = new Mock<IEmployeeRepository>();

        runs.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<PayrollRun>());
        PayrollRun? persisted = null;
        runs.Setup(r => r.AddAsync(It.IsAny<PayrollRun>(), It.IsAny<CancellationToken>()))
            .Callback<PayrollRun, CancellationToken>((pr, _) => persisted = pr)
            .ReturnsAsync(() => persisted!);

        var sut = new PayrollService(runs.Object, items.Object, payslips.Object, employees.Object, new DefaultPayrollCalculator());

        var created = await sut.CreateRun(Guid.NewGuid(), new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31));

        Assert.NotNull(persisted);
        Assert.Equal("Draft", persisted!.Status);
        Assert.True(persisted!.CreatedAtUtc <= DateTime.UtcNow);
        Assert.Equal(created.Id, persisted!.Id);
    }
}
