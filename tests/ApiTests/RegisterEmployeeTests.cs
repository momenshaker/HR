using System.Net;
using System.Net.Http.Json;
using HR.Api.Contracts;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class RegisterEmployeeTests : IClassFixture<AuthenticatedApiFactory>
{
    private readonly AuthenticatedApiFactory _factory;
    public RegisterEmployeeTests(AuthenticatedApiFactory factory) { _factory = factory; }

    [Fact]
    public async Task RegisterEmployee_Succeeds_And_Duplicate_Conflicts()
    {
        var employeeId = Guid.NewGuid();
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
            await repo.AddAsync(new Employee
            {
                Id = employeeId,
                Email = "jane.doe@test.dev",
                FirstName = "Jane",
                LastName = "Doe",
                CreatedAtUtc = DateTime.UtcNow,
                EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date)
            });
        }

        var client = _factory.CreateClient();
        var req = new RegisterEmployeeRequest("jane.doe@test.dev", "jane.doe", "Password123!", employeeId);
        var res = await client.PostAsJsonAsync("/api/v1/auth/register-employee", req);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        // Duplicate
        var dup = await client.PostAsJsonAsync("/api/v1/auth/register-employee", req);
        Assert.Equal(HttpStatusCode.Conflict, dup.StatusCode);
    }
}

