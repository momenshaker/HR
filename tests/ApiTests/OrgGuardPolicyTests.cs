using System.Net;
using System.Net.Http.Headers;
using HR.Api.IntegrationTests;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HR.Api.Tests;

public sealed class OrgGuardPolicyTests : IClassFixture<AuthenticatedApiFactory>
{
    private readonly AuthenticatedApiFactory _factory;

    public OrgGuardPolicyTests(AuthenticatedApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Denies_access_to_other_organization()
    {
        var employeeId = Guid.NewGuid();
        var orgA = new Organization { Id = Guid.NewGuid(), Name = "OrgA", Code = "ORGA" };
        var orgB = new Organization { Id = Guid.NewGuid(), Name = "OrgB", Code = "ORGB" };
        var deptA = new Department { Id = Guid.NewGuid(), OrganizationId = orgA.Id, Name = "DeptA", Code = "DEPA" };

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var orgRepo = scope.ServiceProvider.GetRequiredService<IOrganizationRepository>();
            var deptRepo = scope.ServiceProvider.GetRequiredService<IDepartmentRepository>();
            var empRepo = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
            var empDeptRepo = scope.ServiceProvider.GetRequiredService<IEmployeeDepartmentRepository>();

            await orgRepo.AddAsync(orgA);
            await orgRepo.AddAsync(orgB);
            await deptRepo.AddAsync(deptA);
            await empRepo.AddAsync(new Employee { Id = employeeId, Email = "employee@test.dev", FirstName = "Emp", LastName = "One", CreatedAtUtc = DateTime.UtcNow });
            await empDeptRepo.AssignAsync(employeeId, new[] { deptA.Id });
        }

        var client = _factory.CreateClient();
        var token = JwtTestHelper.CreateToken(
            AuthenticatedApiFactory.Issuer,
            AuthenticatedApiFactory.Audience,
            AuthenticatedApiFactory.SigningKey,
            roles: new[] { "User" },
            additionalClaims: new Dictionary<string, string>
            {
                ["employee_id"] = employeeId.ToString()
            });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Access within employee's org
        var okResponse = await client.GetAsync($"/api/organizations/{orgA.Id}/departments");
        Assert.True(okResponse.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent);

        // Access to other org should be forbidden
        var forbidden = await client.GetAsync($"/api/organizations/{orgB.Id}/departments");
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }
}

