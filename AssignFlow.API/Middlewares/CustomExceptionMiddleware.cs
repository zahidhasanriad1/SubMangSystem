using AssignFlow.Utils.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AssignFlow.API.Middlewares;

public class CustomExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionMiddleware> _logger;

    public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
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
        catch (Exception exception)
        {
            var statusCode = exception switch
            {
                AppException appException => appException.StatusCode,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode >= 500)
                _logger.LogError(exception, "Unhandled request failure. TraceId: {TraceId}", context.TraceIdentifier);
            else
                _logger.LogWarning("Request rejected with status {StatusCode}. TraceId: {TraceId}. Reason: {Reason}", statusCode, context.TraceIdentifier, exception.Message);

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode >= 500 ? "An unexpected error occurred." : exception.Message,
                Detail = statusCode >= 500 ? "Contact support with the supplied trace identifier." : null,
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
