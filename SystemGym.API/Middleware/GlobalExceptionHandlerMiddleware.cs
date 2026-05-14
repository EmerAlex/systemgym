namespace SystemGym.API.Middleware;

using System.Net;
using System.Text.Json;

/// <summary>
/// Middleware centralizado para manejo de excepciones.
/// Reemplaza los 15+ bloques try-catch duplicados en los controllers.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, "Parámetro requerido no proporcionado"),
            ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "No tiene permisos para realizar esta acción"),
            InvalidOperationException invEx => (HttpStatusCode.UnprocessableEntity, invEx.Message),
            _ => (HttpStatusCode.InternalServerError, "Ha ocurrido un error interno. Por favor intente de nuevo.")
        };

        _logger.LogError(exception,
            "Excepción no controlada: {ExceptionType} | Path: {Path} | StatusCode: {StatusCode}",
            exception.GetType().Name,
            context.Request.Path,
            (int)statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            success = false,
            message,
            statusCode = (int)statusCode,
            path = context.Request.Path.Value
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }
}

/// <summary>
/// Extensión para registrar el middleware en el pipeline
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}
