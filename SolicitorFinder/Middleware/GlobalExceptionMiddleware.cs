using FluentValidation;
using System.Net;
using System.Text.Json;

namespace SolicitorFinder.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error occurred. Path: {Path}, Method: {Method}, Errors: {Errors}",
                context.Request.Path, context.Request.Method, ex.Errors.Select(e => e.ErrorMessage));
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error occurred. Path: {Path}, Method: {Method}",
                context.Request.Path, context.Request.Method);
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Invalid request parameters");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found. Path: {Path}, Method: {Method}",
                context.Request.Path, context.Request.Method);
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound, "Resource not found");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation. Path: {Path}, Method: {Method}",
                context.Request.Path, context.Request.Method);
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Invalid operation");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt. Path: {Path}, Method: {Method}, IP: {IP}",
                context.Request.Path, context.Request.Method, context.Connection.RemoteIpAddress);
            await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized, "Unauthorized access");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Request cancelled. Path: {Path}, Method: {Method}",
                context.Request.Path, context.Request.Method);
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Request cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception occurred. Path: {Path}, Method: {Method}, Query: {Query}, IP: {IP}",
                context.Request.Path,
                context.Request.Method,
                context.Request.QueryString,
                context.Connection.RemoteIpAddress);
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "An internal server error occurred");
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new ValidationErrorResponse
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = "One or more validation errors occurred",
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Detail = _environment.IsDevelopment() ? exception.Message : null,
            StackTrace = _environment.IsDevelopment() ? exception.StackTrace : null,
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private sealed class ValidationErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public Dictionary<string, string[]> Errors { get; init; } = new();
        public DateTime Timestamp { get; init; }
    }

    private sealed class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? Detail { get; init; }
        public string? StackTrace { get; init; }
        public DateTime Timestamp { get; init; }
    }
}
