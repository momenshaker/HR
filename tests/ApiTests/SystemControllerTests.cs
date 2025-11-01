using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class SystemControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.WithWebHostBuilder(builder =>
    {
        builder.UseSetting("DOTNET_ENVIRONMENT", "Development");
    }).CreateClient();

    [Fact]
    public async Task Health_ReturnsHealthyStatus()
    {
        var response = await _client.GetAsync("/api/v1/health").ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SystemHealthResponse>().ConfigureAwait(false);

        Assert.NotNull(payload);
        Assert.Equal("Healthy", payload!.Status);
        Assert.False(string.IsNullOrWhiteSpace(payload.Environment));
    }

    [Fact]
    public async Task Version_ReturnsAssemblyVersion()
    {
        var response = await _client.GetAsync("/api/v1/version").ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SystemVersionResponse>().ConfigureAwait(false);

        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.Version));
        Assert.Equal(typeof(Program).Assembly.GetName().Version?.ToString(), payload.Version);
        Assert.False(string.IsNullOrWhiteSpace(payload.Environment));
    }

    private sealed record SystemHealthResponse(string Status, string Environment, DateTimeOffset Timestamp);

    private sealed record SystemVersionResponse(string Version, string Environment);
}
