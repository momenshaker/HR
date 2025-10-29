using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HR.Api.Swagger;

/// <summary>
///     Configures Swagger documents for each discovered API version.
/// </summary>
public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "HR Platform Public API",
                Version = description.ApiVersion.ToString(),
                Description = "Secure, versioned HR platform API.",
                Contact = new OpenApiContact
                {
                    Name = "Hercules IT Solutions",
                    Email = "info@herculesit.com",
                    Url = new Uri("https://www.herculesit.com")
                },
                License = new OpenApiLicense
                {
                    Name = "Proprietary",
                    Url = new Uri("https://www.herculesit.com/legal")
                }
            });
        }
    }
}
