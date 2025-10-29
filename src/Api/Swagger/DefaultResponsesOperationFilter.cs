using System.Collections.Generic;
using System.Globalization;
using HR.Api.Contracts;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HR.Api.Swagger;

/// <summary>
///     Adds the default security and error responses to all documented operations.
/// </summary>
public sealed class DefaultResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        EnsureResponse(operation, StatusCodes.Status401Unauthorized, "Unauthorized");
        EnsureResponse(operation, StatusCodes.Status403Forbidden, "Forbidden");
        EnsureResponse(operation, StatusCodes.Status422UnprocessableEntity, "Validation failed");
        EnsureResponse(operation, StatusCodes.Status429TooManyRequests, "Rate limited");

        foreach (var response in operation.Responses.Values)
        {
            if (!response.Content.ContainsKey("application/json"))
            {
                continue;
            }

            response.Content["application/json"].Schema = context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository);
        }
    }

    private static void EnsureResponse(OpenApiOperation operation, int statusCode, string description)
    {
        var key = statusCode.ToString(CultureInfo.InvariantCulture);
        if (!operation.Responses.ContainsKey(key))
        {
            operation.Responses.Add(key, new OpenApiResponse
            {
                Description = description,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/json",
                        new OpenApiMediaType()
                    }
                }
            });
        }
    }
}
