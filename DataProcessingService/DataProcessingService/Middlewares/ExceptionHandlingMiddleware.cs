using System.Net;
using System.Text.Json;

namespace DataProcessingService.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Request was cancelled by client.");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                error = "An unexpected error occurred.",
                details = ex.Message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}