using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace HR.Api.IntegrationTests;

public sealed class AuthenticatedApiFactory : WebApplicationFactory<Program>
{
    public const string Issuer = "https://tests";
    public const string Audience = "hr-api-tests";
    public const string SigningKey = "test-super-secret-key-1234567890";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("DOTNET_ENVIRONMENT", "Development");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["HrPlatform:Data:RepositoryProvider"] = "InMemory",
                ["Jwt:Issuer"] = Issuer,
                ["Jwt:Audience"] = Audience,
                ["Jwt:Key"] = SigningKey,
                ["Jwt:CustomerClaim"] = "cust",
                ["Authentication:TokenLifetimeMinutes"] = "60",
                ["Authentication:Users:0:Id"] = Guid.NewGuid().ToString(),
                ["Authentication:Users:0:Email"] = "admin@tests.dev",
                ["Authentication:Users:0:Password"] = "Password123!",
                ["Authentication:Users:0:Roles:0"] = "Admin",
                ["Authentication:Users:0:Roles:1"] = "HR",
                ["Authentication:Users:0:CustomerId"] = "test-tenant",
                ["RateLimit:RequestsPerWindow"] = "3",
                ["RateLimit:WindowSeconds"] = "60",
                ["Idempotency:WindowHours"] = "24"
            });
        });
    }
}
