using System.Net;
using System.Net.Http.Json;
using HR.Api.Contracts;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class ErrorMappingTests : IClassFixture<AuthenticatedApiFactory>
{
    private readonly AuthenticatedApiFactory _factory;

    public ErrorMappingTests(AuthenticatedApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Returns_json_error_payload_for_404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/this/route/does/not/exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(payload);
        Assert.Equal("not_found", payload!.Code);
        Assert.False(string.IsNullOrWhiteSpace(payload.Message));
    }
}

