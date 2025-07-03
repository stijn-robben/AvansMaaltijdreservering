using System.Net;
using System.Text.Json;
using AvansMaaltijdreservering.Core.Domain.Exceptions;

namespace AvansMaaltijdreservering.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case StudentBlockedException blockedEx:
                response.Message = blockedEx.Message;
                response.Details = $"Student {blockedEx.StudentId} has {blockedEx.NoShowCount} no-shows";
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                _logger.LogWarning("Blocked student attempted action: {StudentId} with {NoShowCount} no-shows", 
                    blockedEx.StudentId, blockedEx.NoShowCount);
                break;

            case ReservationException reservationEx:
                response.Message = reservationEx.Message;
                response.Details = $"Package: {reservationEx.PackageId}, Student: {reservationEx.StudentId}";
                response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                _logger.LogWarning("Reservation conflict: Package {PackageId}, Student {StudentId} - {Message}", 
                    reservationEx.PackageId, reservationEx.StudentId, reservationEx.Message);
                break;

            case BusinessRuleException businessEx:
                response.Message = businessEx.Message;
                response.Details = businessEx.RuleName;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogWarning("Business rule violation: {Rule} - {Message}", businessEx.RuleName, businessEx.Message);
                break;

            case ArgumentException argEx:
                response.Message = argEx.Message;
                response.Details = "Invalid argument provided";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogWarning("Invalid argument: {Message}", argEx.Message);
                break;

            case UnauthorizedAccessException unauthorizedEx:
                response.Message = unauthorizedEx.Message;
                response.Details = "Access denied";
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                _logger.LogWarning("Unauthorized access attempt: {Message}", unauthorizedEx.Message);
                break;

            case InvalidOperationException invalidOpEx:
                response.Message = invalidOpEx.Message;
                response.Details = "Invalid operation";
                response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                _logger.LogWarning("Invalid operation: {Message}", invalidOpEx.Message);
                break;

            default:
                response.Message = "An internal server error occurred";
                response.Details = "Please try again later";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }

        response.TraceId = context.TraceIdentifier;
        response.Timestamp = DateTime.UtcNow;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public int StatusCode { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}