using System.Net;
using System.Net.Http.Headers;
using HR.Api.IntegrationTests;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HR.Api.Tests;

public sealed class EmployeeSelfPolicyTests : IClassFixture<AuthenticatedApiFactory>
{
    private readonly AuthenticatedApiFactory _factory;
    public EmployeeSelfPolicyTests(AuthenticatedApiFactory factory) { _factory = factory; }

    [Fact]
    public async Task EmployeeSelf_Allows_Self_Denies_Other()
    {
        var employeeA = new Employee { Id = Guid.NewGuid(), Email = "a@test.dev", FirstName = "A", LastName = "User", CreatedAtUtc = DateTime.UtcNow, EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        var employeeB = new Employee { Id = Guid.NewGuid(), Email = "b@test.dev", FirstName = "B", LastName = "User", CreatedAtUtc = DateTime.UtcNow, EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
            await repo.AddAsync(employeeA);
            await repo.AddAsync(employeeB);
        }

        var client = _factory.CreateClient();
        var token = JwtTestHelper.CreateToken(AuthenticatedApiFactory.Issuer, AuthenticatedApiFactory.Audience, AuthenticatedApiFactory.SigningKey, Array.Empty<string>(), new Dictionary<string, string> { ["employee_id"] = employeeA.Id.ToString() });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var ok = await client.GetAsync($"/api/employees/{employeeA.Id}");
        Assert.NotEqual(HttpStatusCode.Forbidden, ok.StatusCode);

        var forbidden = await client.GetAsync($"/api/employees/{employeeB.Id}");
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }
}

