using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using HR.Api.Contracts;
using HR.Api.Filters;
using HR.Api.Idempotency;
using HR.Api.Middleware;
using HR.Api.Swagger;
using HR.Api.Validation;
using HR.Infrastructure.Extensions;
using HR.Infrastructure.Options;
using HR.Infrastructure.Persistence.EntityFramework;
using HR.Infrastructure.Security.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(AuthenticationOptions.SectionName));
builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection(RateLimitOptions.SectionName));
builder.Services.Configure<IdempotencyOptions>(builder.Configuration.GetSection(IdempotencyOptions.SectionName));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IIdempotencyStore, InMemoryIdempotencyStore>();
builder.Services.AddScoped<AuditLoggingFilter>();

builder.Services.AddRateLimiter(options =>
{
    var rateOptions = builder.Configuration.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>() ?? new RateLimitOptions();
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var token = context.User?.FindFirst("sub")?.Value
                    ?? context.Request.Headers.Authorization.ToString()
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "anonymous";

        return RateLimitPartition.GetTokenBucketLimiter(token, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = rateOptions.RequestsPerWindow,
            QueueLimit = rateOptions.RequestsPerWindow,
            ReplenishmentPeriod = TimeSpan.FromSeconds(rateOptions.WindowSeconds),
            TokensPerPeriod = rateOptions.RequestsPerWindow,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = rateOptions.WindowSeconds.ToString(CultureInfo.InvariantCulture);
        var payload = new ErrorResponse("rate_limited", "Too many requests.", context.HttpContext.TraceIdentifier);
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(payload, cancellationToken: cancellationToken).ConfigureAwait(false);
    };
});

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var optionsMonitor = context.HttpContext.RequestServices.GetRequiredService<IOptions<JwtOptions>>();
                var configuredOptions = optionsMonitor.Value;

                if (!context.Principal!.Claims.Any(claim => string.Equals(claim.Type, configuredOptions.CustomerClaim, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Fail($"Token is missing required '{configuredOptions.CustomerClaim}' claim.");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
        options.Filters.AddService<AuditLoggingFilter>();
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status401Unauthorized));
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status403Forbidden));
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity));
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests));
    })
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressInferBindingSourcesForParameters = true;
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => new ErrorDetail(entry.Key, error.ErrorMessage)))
            .ToArray();

        var response = new ErrorResponse("validation_failed", "One or more validation errors occurred.", context.HttpContext.TraceIdentifier)
        {
            Details = errors
        };

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity
        };
    };
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped(typeof(IValidator<>), typeof(DataAnnotationsValidator<>));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var bearerScheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        },
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Input your JWT token in the format: Bearer {token}."
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { bearerScheme, Array.Empty<string>() }
    });
    options.OperationFilter<DefaultResponsesOperationFilter>();
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddHrPlatform(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetService<HrDbContext>();
    if (dbContext is not null)
    {
        await dbContext.Database.MigrateAsync();
    }

    await IdentitySeeder.SeedAsync(scope.ServiceProvider, CancellationToken.None);
}

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"HR Platform API {description.GroupName.ToUpperInvariant()}");
        }
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<IdempotencyMiddleware>();
app.UseMiddleware<EtagMiddleware>();
app.UseMiddleware<SubscriptionGuardMiddleware>();
app.MapControllers();

app.Run();

public partial class Program;
