namespace AllTheBeans.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var start = DateTime.UtcNow;
        string method = context.Request.Method;
        string path = context.Request.Path;
        _logger.LogInformation("Started {Method} {Path}", method, path);

        await _next(context);

        var status = context.Response.StatusCode;
        var elapsedMs = (DateTime.UtcNow - start).TotalMilliseconds;
        _logger.LogInformation("Finished {Method} {Path} with {StatusCode} in {Elapsed:0.00}ms",
                                method, path, status, elapsedMs);
    }
}
