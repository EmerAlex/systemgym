namespace SystemGym.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Serilog;

/// <summary>
/// Controlador base para todos los controladores de la API
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger<BaseController> Logger;
    protected readonly IMediator Mediator;

    protected BaseController(ILogger<BaseController> logger, IMediator mediator)
    {
        Logger = logger;
        Mediator = mediator;
    }

    protected IActionResult CreatedResult(Guid resourceId, string? message = null, object? data = null)
    {
        var response = new
        {
            success = true,
            message = message ?? "Recurso creado exitosamente",
            data = new { id = resourceId, resource = data }
        };

        return CreatedAtAction(null, response);
    }

    protected IActionResult OkResult(object? data = null, string? message = null)
    {
        var response = new
        {
            success = true,
            message = message ?? "Operación exitosa",
            data
        };

        return Ok(response);
    }

    protected IActionResult BadRequestResult(string message, Dictionary<string, string[]>? errors = null)
    {
        var response = new
        {
            success = false,
            message,
            errors
        };

        return BadRequest(response);
    }

    protected IActionResult NotFoundResult(string message)
    {
        var response = new
        {
            success = false,
            message
        };

        return NotFound(response);
    }

    protected IActionResult InternalServerErrorResult(string message)
    {
        var response = new
        {
            success = false,
            message
        };

        Logger.LogError(message);
        return StatusCode(500, response);
    }

    /// <summary>
    /// Extrae el UserId del contexto del usuario autenticado
    /// </summary>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidOperationException("No se pudo obtener el ID del usuario autenticado");

        return userId;
    }
}
