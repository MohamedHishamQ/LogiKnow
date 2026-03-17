using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace LogiKnow.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await WriteProblemDetails(context, HttpStatusCode.NotFound, "Not Found", ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await WriteProblemDetails(context, HttpStatusCode.Forbidden, "Forbidden", ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            await WriteProblemDetails(context, HttpStatusCode.BadRequest, "Bad Request", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblemDetails(context, HttpStatusCode.InternalServerError,
                "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetails(HttpContext context, HttpStatusCode status, string title, string detail)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}
