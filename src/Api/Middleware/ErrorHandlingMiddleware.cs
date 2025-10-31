using System.Linq;
using FluentValidation;
using HR.Api.Contracts;
using HR.Application.Common.Exceptions;
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
        catch (UniqueConstraintViolationException uniqueException)
        {
            await WriteConflictAsync(context, uniqueException).ConfigureAwait(false);
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
            .Select(error => new ErrorDetail(error.PropertyName, error.ErrorMessage, error.ErrorCode))
            .ToArray();

        return WriteErrorAsync(
            context,
            StatusCodes.Status422UnprocessableEntity,
            "validation_failed",
            "One or more validation errors occurred.",
            details);
    }

    private Task WriteConflictAsync(HttpContext context, UniqueConstraintViolationException exception)
    {
        var details = new[]
        {
            new ErrorDetail(exception.Field, $"Value '{exception.Value}' already exists.", "UniqueViolation")
        };

        return WriteErrorAsync(context, StatusCodes.Status409Conflict, "conflict", exception.Message, details);
    }

    private Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        IReadOnlyCollection<ErrorDetail>? details = null)
    {
        var response = new ErrorResponse(code, message, context.TraceIdentifier)
        {
            Details = details ?? Array.Empty<ErrorDetail>()
        };
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(response);
    }
}
