using System.Linq;
using FluentValidation;
using HR.Api.Contracts;
using Microsoft.Extensions.Logging;

namespace HR.Api.Middleware;

/// <summary>
///     Middleware responsible for translating unhandled exceptions into standardized error responses.
/// </summary>
public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (ValidationException validationException)
        {
            await WriteValidationFailureAsync(context, validationException).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred during request processing.");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "internal_server_error", "An unexpected error occurred.").ConfigureAwait(false);
        }
    }

    private Task WriteValidationFailureAsync(HttpContext context, ValidationException validationException)
    {
        var details = validationException.Errors
            .Select(error => new ErrorDetail(error.PropertyName, error.ErrorMessage))
            .ToArray();

        var response = new ErrorResponse("validation_failed", "One or more validation errors occurred.", context.TraceIdentifier)
        {
            Details = details
        };

        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(response);
    }

    private Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message)
    {
        var response = new ErrorResponse(code, message, context.TraceIdentifier);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(response);
    }
}
