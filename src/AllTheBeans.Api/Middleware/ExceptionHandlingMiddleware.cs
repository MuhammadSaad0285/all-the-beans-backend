using AllTheBeans.Application.Errors;
using System.Text.Json;

namespace AllTheBeans.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            _logger.LogWarning(ex, "Not found error");
            await WriteErrorResponse(context, ex.Message);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            _logger.LogInformation(ex, "Validation error");
            await WriteErrorResponse(context, ex.Message);
        }
        catch (DuplicateRequestException ex)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            _logger.LogInformation(ex, "Duplicate request");
            await WriteErrorResponse(context, ex.Message);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError(ex, "Unexpected server error");
            await WriteErrorResponse(context, "An unexpected error occurred.");
        }
    }

    private static Task WriteErrorResponse(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";
        var errorObj = new { error = message };
        return context.Response.WriteAsync(JsonSerializer.Serialize(errorObj));
    }
}
