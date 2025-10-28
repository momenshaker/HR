using System;
using HR.Infrastructure.Extensions;
using HR.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v0.4.0",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "HR Platform Public API",
            Version = "v0.4.0",
            Description =
                "Public endpoints for system status, authentication, subscriptions, billing plans, and audit visibility.",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Hercules IT Solutions",
                Email = "info@herculesit.com",
                Url = new Uri("https://www.herculesit.com")
            },
            License = new Microsoft.OpenApi.Models.OpenApiLicense
            {
                Name = "Proprietary",
                Url = new Uri("https://www.herculesit.com/legal")
            }
        });

    var bearerScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Id = "Bearer",
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
        },
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Input your JWT token in the format: Bearer {token}."
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);
    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                bearerScheme,
                Array.Empty<string>()
            }
        });
});
builder.Services.AddHrPlatform(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetService<HrDbContext>();
    if (dbContext is not null)
    {
        await dbContext.Database.MigrateAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
