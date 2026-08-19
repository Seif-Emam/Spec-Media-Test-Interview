using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using TestIjnterview.Common.Models;

namespace TestIjnterview.Common.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        int statusCode;
        string message;
        IDictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                message = "Validation failed for the request.";
                errors = validationException.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.ToArray());
                break;

            case KeyNotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                message = notFoundEx.Message;
                break;

            case InvalidOperationException invalidOpEx:
                statusCode = StatusCodes.Status400BadRequest;
                message = invalidOpEx.Message;
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                message = "An unexpected server error occurred.";
                break;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(message, statusCode, errors);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
