using System.Net;
using System.Net.Http.Json;
using HR.Api.Contracts;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class AuthControllerTests(AuthenticatedApiFactory factory) : IClassFixture<AuthenticatedApiFactory>
{
    private readonly AuthenticatedApiFactory _factory = factory;

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAccessToken()
    {
        using var client = _factory.CreateClient();
        var request = new LoginRequest("admin@tests.dev", "Password123!");

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>().ConfigureAwait(false);

        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.AccessToken));
        Assert.Equal("Bearer", payload.TokenType);
        Assert.True(payload.ExpiresIn > 0);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var request = new LoginRequest("admin@tests.dev", "WrongPassword");

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
        Assert.NotNull(error);
        Assert.Equal("invalid_credentials", error!.Code);
    }
}
