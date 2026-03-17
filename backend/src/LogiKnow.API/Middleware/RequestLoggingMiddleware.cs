using System.Diagnostics;

namespace LogiKnow.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogDebug("→ {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context);

        sw.Stop();
        _logger.LogDebug("← {Method} {Path} → {StatusCode} ({ElapsedMs}ms)",
            context.Request.Method, context.Request.Path,
            context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
