using DataProcessingService.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DataProcessingService.Infrastructure;

public static class GlobalExceptionHandler
{
    public static async Task HandleAsync(HttpContext context)
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception");

        var (statusCode, title, details) = exception switch
        {
            OperationCanceledException => (StatusCodes.Status499ClientClosedRequest, "Request was cancelled by the client", null),
            ValidationFailedException validationEx => (StatusCodes.Status400BadRequest, "Validation failed", validationEx.Errors),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null)
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        if (details is { } validationErrors)
        {
            var validationProblem = new ValidationProblemDetails(validationErrors)
            {
                Status = statusCode,
                Title = title,
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(validationProblem);
        }
        else
        {
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception?.Message,
                Instance = context.Request.Path
            };
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}