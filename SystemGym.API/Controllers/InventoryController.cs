namespace SystemGym.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using SystemGym.Application.DTOs.Inventory;
using SystemGym.Application.Features.Inventory.Commands;
using SystemGym.Application.Features.Inventory.Queries;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Controlador para gestionar inventario
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InventoryController : BaseController
{
    public InventoryController(ILogger<InventoryController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    /// <summary>
    /// Ajustar inventario de un producto
    /// </summary>
    [HttpPost("adjust")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdjustInventory(
        [FromBody] AdjustInventoryDto adjustDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();

            var command = new AdjustInventoryCommand
            {
                ProductId = adjustDto.ProductId,
                UserId = userId,
                Cambio = adjustDto.Cambio,
                MotivoAuditoria = adjustDto.MotivoAuditoria
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success && result.Errors?.ContainsKey("ProductId") == true)
                return NotFoundResult(result.Message ?? "Producto no encontrado");

            if (!result.Success)
                return BadRequestResult(result.Message ?? "No fue posible ajustar el inventario", result.Errors);

            Logger.LogInformation(
                "Ajuste de inventario persistido: Producto {ProductId}, Cambio {Cambio}, LogId {LogId}",
                adjustDto.ProductId,
                adjustDto.Cambio,
                result.LogId);

            return OkResult(new
            {
                productId = result.ProductId,
                logId = result.LogId,
                cantidadAnterior = result.CantidadAnterior,
                cantidadNueva = result.CantidadNueva,
                diferencia = result.Diferencia,
                operacion = result.Operacion
            }, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al ajustar inventario");
            return InternalServerErrorResult("Error al ajustar el inventario");
        }
    }

    /// <summary>
    /// Obtener logs de inventario (solo Admin)
    /// </summary>
    [HttpGet("logs")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInventoryLogs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? productId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetInventoryLogsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                ProductId = productId
            };

            var result = await Mediator.Send(query, cancellationToken);

            Logger.LogInformation("Logs de inventario obtenidos: {Count} items", result.Data.Count);

            return OkResult(result, "Logs de inventario obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener logs de inventario");
            return InternalServerErrorResult("Error al obtener los logs de inventario");
        }
    }

    /// <summary>
    /// Obtener logs de inventario para un producto (solo Admin)
    /// </summary>
    [HttpGet("logs/product/{productId}")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProductInventoryLogs(
        [FromRoute] Guid productId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetProductInventoryLogsQuery
            {
                ProductId = productId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await Mediator.Send(query, cancellationToken);

            Logger.LogInformation("Logs de inventario del producto obtenidos: {Count} items", result.Data.Count);

            return OkResult(result, "Logs de inventario del producto obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener logs de inventario del producto {ProductId}", productId);
            return InternalServerErrorResult("Error al obtener los logs de inventario");
        }
    }
}
