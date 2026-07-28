using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            var (status, title, detail) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Request conflict", exception.Message),
                DbUpdateException => (StatusCodes.Status409Conflict, "Database conflict", "The operation conflicts with existing data."),
                _ => (StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred.")
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            });
        }
    }
}
