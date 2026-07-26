using System.Net;
using System.Text.Json;
using TaskManager.Application.Exceptions;

namespace TaskManager.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled API exception");
            await WriteErrorResponseAsync(context, exception);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception is DuplicateResourceException
            ? (int)HttpStatusCode.Conflict
            : (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = exception is DuplicateResourceException
                ? exception.Message
                : "An unexpected error occurred.",
            detail = exception.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
