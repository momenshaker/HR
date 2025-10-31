using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HR.Api.Contracts;
using HR.Application.DTOs;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class DepartmentsControllerTests : IDisposable
{
    private readonly AuthenticatedApiFactory _factory = new();
    private readonly string _accessToken = JwtTestHelper.CreateToken(
        AuthenticatedApiFactory.Issuer,
        AuthenticatedApiFactory.Audience,
        AuthenticatedApiFactory.SigningKey,
        new[] { "Admin" });

    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task GetHierarchy_ReturnsMaterializedPathTree()
    {
        using var client = CreateClient();
        var organization = await CreateOrganizationAsync(client, "Acme Org", "ACME").ConfigureAwait(false);

        var headOffice = await CreateDepartmentAsync(client, organization.Id, "Head Office", "HQ", null).ConfigureAwait(false);
        var engineering = await CreateDepartmentAsync(client, organization.Id, "Engineering", "ENG", headOffice.Id).ConfigureAwait(false);
        var platform = await CreateDepartmentAsync(client, organization.Id, "Platform", "PLAT", engineering.Id).ConfigureAwait(false);

        var response = await client
            .GetAsync($"/api/organizations/{organization.Id}/departments?hierarchy=true")
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var hierarchy = await response.Content.ReadFromJsonAsync<DepartmentDto[]>().ConfigureAwait(false);

        Assert.NotNull(hierarchy);
        var root = Assert.Single(hierarchy!);
        Assert.Equal(headOffice.Id, root.Id);
        Assert.Equal($"/org/{organization.Id}/dept/{headOffice.Id}", root.Path);
        Assert.Equal(0, root.Level);

        var engineeringNode = Assert.Single(root.Children);
        Assert.Equal(engineering.Id, engineeringNode.Id);
        Assert.Equal($"{root.Path}/{engineering.Id}", engineeringNode.Path);
        Assert.Equal(1, engineeringNode.Level);

        var platformNode = Assert.Single(engineeringNode.Children);
        Assert.Equal(platform.Id, platformNode.Id);
        Assert.Equal($"{engineeringNode.Path}/{platform.Id}", platformNode.Path);
        Assert.Equal(2, platformNode.Level);
    }

    [Fact]
    public async Task MoveDepartment_ReparentsSubtreeAndUpdatesLevels()
    {
        using var client = CreateClient();
        var organization = await CreateOrganizationAsync(client, "Delta Org", "DELTA").ConfigureAwait(false);

        var headOffice = await CreateDepartmentAsync(client, organization.Id, "Head Office", "HQ", null).ConfigureAwait(false);
        var engineering = await CreateDepartmentAsync(client, organization.Id, "Engineering", "ENG", headOffice.Id).ConfigureAwait(false);
        var platform = await CreateDepartmentAsync(client, organization.Id, "Platform", "PLAT", engineering.Id).ConfigureAwait(false);
        var operations = await CreateDepartmentAsync(client, organization.Id, "Operations", "OPS", null).ConfigureAwait(false);

        var moveResponse = await client
            .PostAsJsonAsync(
                $"/api/organizations/{organization.Id}/departments/{engineering.Id}:move",
                new MoveDepartmentRequest { NewParentDepartmentId = operations.Id })
            .ConfigureAwait(false);

        moveResponse.EnsureSuccessStatusCode();
        var moved = await moveResponse.Content.ReadFromJsonAsync<DepartmentDto>().ConfigureAwait(false);

        Assert.NotNull(moved);
        Assert.Equal(operations.Id, moved!.ParentDepartmentId);
        Assert.Equal($"/org/{organization.Id}/dept/{operations.Id}/{engineering.Id}", moved.Path);
        Assert.Equal(operations.Level + 1, moved.Level);

        var platformResponse = await client
            .GetAsync($"/api/organizations/{organization.Id}/departments/{platform.Id}")
            .ConfigureAwait(false);

        platformResponse.EnsureSuccessStatusCode();
        var platformDto = await platformResponse.Content.ReadFromJsonAsync<DepartmentDto>().ConfigureAwait(false);

        Assert.NotNull(platformDto);
        Assert.Equal(engineering.Id, platformDto!.ParentDepartmentId);
        Assert.Equal($"{moved.Path}/{platform.Id}", platformDto.Path);
        Assert.Equal(moved.Level + 1, platformDto.Level);
    }

    [Fact]
    public async Task MoveDepartment_ToDescendant_ReturnsValidationError()
    {
        using var client = CreateClient();
        var organization = await CreateOrganizationAsync(client, "Gamma Org", "GAMMA").ConfigureAwait(false);

        var headOffice = await CreateDepartmentAsync(client, organization.Id, "Head Office", "HQ", null).ConfigureAwait(false);
        var engineering = await CreateDepartmentAsync(client, organization.Id, "Engineering", "ENG", headOffice.Id).ConfigureAwait(false);
        var platform = await CreateDepartmentAsync(client, organization.Id, "Platform", "PLAT", engineering.Id).ConfigureAwait(false);

        var response = await client
            .PostAsJsonAsync(
                $"/api/organizations/{organization.Id}/departments/{headOffice.Id}:move",
                new MoveDepartmentRequest { NewParentDepartmentId = platform.Id })
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);

        Assert.NotNull(error);
        Assert.Equal("validation_failed", error!.Code);
        Assert.Contains(error.Details, detail => detail.Code == "HierarchyCycle");
    }

    private HttpClient CreateClient()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        return client;
    }

    private static async Task<OrganizationDto> CreateOrganizationAsync(HttpClient client, string name, string code)
    {
        var request = new CreateOrganizationRequest
        {
            Name = name,
            Code = code,
            Description = $"{name} description",
            IsActive = true
        };

        var response = await client.PostAsJsonAsync("/api/organizations", request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OrganizationDto>().ConfigureAwait(false))!;
    }

    private static async Task<DepartmentDto> CreateDepartmentAsync(
        HttpClient client,
        Guid organizationId,
        string name,
        string code,
        Guid? parentDepartmentId)
    {
        var request = new CreateDepartmentRequest
        {
            OrganizationId = organizationId,
            Name = name,
            Code = code,
            ParentDepartmentId = parentDepartmentId,
            Branch = "Corporate",
            Location = "Remote",
            Description = $"{name} department",
            IsActive = true
        };

        var response = await client
            .PostAsJsonAsync($"/api/organizations/{organizationId}/departments", request)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DepartmentDto>().ConfigureAwait(false))!;
    }
}
