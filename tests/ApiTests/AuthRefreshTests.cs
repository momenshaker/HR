using System.Net;
using System.Net.Http.Json;
using HR.Api.Contracts;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class AuthRefreshTests : IClassFixture<AuthenticatedApiFactory>
{
    private readonly AuthenticatedApiFactory _factory;
    public AuthRefreshTests(AuthenticatedApiFactory factory) { _factory = factory; }

    [Fact]
    public async Task Refresh_RotatesToken_OldTokenRejected()
    {
        var client = _factory.CreateClient();

        var login = new LoginRequest("admin@tests.dev", "Password123!");
        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", login);
        loginResponse.EnsureSuccessStatusCode();
        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.RefreshToken));

        var refreshRequest = new RefreshRequest(auth.RefreshToken!);
        var refreshResponse = await client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);
        refreshResponse.EnsureSuccessStatusCode();
        var refreshed = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(refreshed);
        Assert.NotEqual(auth.RefreshToken, refreshed!.RefreshToken);

        // Using old token should now be invalid
        var invalidResponse = await client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, invalidResponse.StatusCode);
    }
}

