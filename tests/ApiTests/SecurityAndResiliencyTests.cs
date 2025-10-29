using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HR.Api.Contracts;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HR.Api.IntegrationTests;

public sealed class SecurityAndResiliencyTests(AuthenticatedApiFactory factory) : IClassFixture<AuthenticatedApiFactory>
{
    private readonly AuthenticatedApiFactory _factory = factory;

    [Fact]
    public async Task Employees_Unauthenticated_Returns401()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/Employees").ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Employees_WithEmployeeRole_Returns403()
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Employees");
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTestHelper.CreateToken(AuthenticatedApiFactory.Issuer, AuthenticatedApiFactory.Audience, AuthenticatedApiFactory.SigningKey, new[] { "Employee" })
        );

        var response = await client.SendAsync(request).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateDepartment_InvalidPayload_Returns422()
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Departments")
        {
            Content = JsonContent.Create(new { Name = "", Code = "" })
        };
        request.Headers.Authorization = BuildAdminAuthorizationHeader();
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await client.SendAsync(request).ConfigureAwait(false);

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
        Assert.NotNull(error);
        Assert.Equal("validation_failed", error!.Code);
        Assert.NotEmpty(error.Details);
    }

    [Fact]
    public async Task CreateDepartment_WithIdempotencyKey_ReplaysCachedResponse()
    {
        using var client = _factory.CreateClient();
        var payload = new { Name = "Operations", Code = "OPS" };
        var idempotencyKey = Guid.NewGuid().ToString();
        var token = JwtTestHelper.CreateToken(AuthenticatedApiFactory.Issuer, AuthenticatedApiFactory.Audience, AuthenticatedApiFactory.SigningKey, new[] { "Admin" });

        using var initialRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Departments")
        {
            Content = JsonContent.Create(payload)
        };
        initialRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        initialRequest.Headers.Add("Idempotency-Key", idempotencyKey);

        var firstResponse = await client.SendAsync(initialRequest).ConfigureAwait(false);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        var created = await firstResponse.Content.ReadFromJsonAsync<DepartmentDto>().ConfigureAwait(false);
        Assert.NotNull(created);

        using var replayRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Departments")
        {
            Content = JsonContent.Create(payload)
        };
        replayRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        replayRequest.Headers.Add("Idempotency-Key", idempotencyKey);

        var replayResponse = await client.SendAsync(replayRequest).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.Created, replayResponse.StatusCode);
        Assert.Equal("true", replayResponse.Headers.GetValues("Idempotency-Replayed").Single());
        var replayedBody = await replayResponse.Content.ReadFromJsonAsync<DepartmentDto>().ConfigureAwait(false);
        Assert.Equal(created!.Id, replayedBody!.Id);
    }

    [Fact]
    public async Task Departments_RequestExceedingRateLimit_Returns429()
    {
        using var client = _factory.CreateClient();
        var token = JwtTestHelper.CreateToken(AuthenticatedApiFactory.Issuer, AuthenticatedApiFactory.Audience, AuthenticatedApiFactory.SigningKey, new[] { "Admin" });

        for (var i = 0; i < 3; i++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Departments");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(request).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        using var throttledRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Departments");
        throttledRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var throttledResponse = await client.SendAsync(throttledRequest).ConfigureAwait(false);

        Assert.Equal((HttpStatusCode)429, throttledResponse.StatusCode);
        Assert.True(throttledResponse.Headers.Contains("Retry-After"));
        var error = await throttledResponse.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
        Assert.Equal("rate_limited", error!.Code);
    }

    [Fact]
    public async Task GetDepartment_WithMatchingEtag_Returns304()
    {
        using var client = _factory.CreateClient();
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Departments")
        {
            Content = JsonContent.Create(new { Name = "Finance", Code = "FIN" })
        };
        createRequest.Headers.Authorization = BuildAdminAuthorizationHeader();
        createRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var createResponse = await client.SendAsync(createRequest).ConfigureAwait(false);
        createResponse.EnsureSuccessStatusCode();
        var department = await createResponse.Content.ReadFromJsonAsync<DepartmentDto>().ConfigureAwait(false);
        Assert.NotNull(department);

        using var initialGet = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Departments/{department!.Id}");
        initialGet.Headers.Authorization = BuildAdminAuthorizationHeader();
        var initialResponse = await client.SendAsync(initialGet).ConfigureAwait(false);
        initialResponse.EnsureSuccessStatusCode();
        var etag = initialResponse.Headers.ETag?.Tag;
        Assert.False(string.IsNullOrWhiteSpace(etag));

        using var cacheRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Departments/{department.Id}");
        cacheRequest.Headers.Authorization = BuildAdminAuthorizationHeader();
        cacheRequest.Headers.TryAddWithoutValidation("If-None-Match", etag);

        var cacheResponse = await client.SendAsync(cacheRequest).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.NotModified, cacheResponse.StatusCode);
    }

    [Fact]
    public async Task CreateDepartment_WritesAuditLogEntry()
    {
        var auditFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<TestAuditLogger>();
                services.AddSingleton<IAuditLogger>(sp => sp.GetRequiredService<TestAuditLogger>());
            });
        });

        using var client = auditFactory.CreateClient();
        var auditLogger = auditFactory.Services.GetRequiredService<TestAuditLogger>();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Departments")
        {
            Content = JsonContent.Create(new { Name = "Compliance", Code = "CMP" })
        };
        request.Headers.Authorization = BuildAdminAuthorizationHeader();
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await client.SendAsync(request).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        Assert.Contains(auditLogger.Entries, entry => entry.Entity == "Department" && entry.Action == "create");
    }

    private static AuthenticationHeaderValue BuildAdminAuthorizationHeader()
    {
        var token = JwtTestHelper.CreateToken(AuthenticatedApiFactory.Issuer, AuthenticatedApiFactory.Audience, AuthenticatedApiFactory.SigningKey, new[] { "Admin" });
        return new AuthenticationHeaderValue("Bearer", token);
    }
}
