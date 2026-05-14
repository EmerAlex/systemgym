namespace SystemGym.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using SystemGym.Application.Features.Sales.Commands;
using SystemGym.Application.Features.Sales.Queries;
using SystemGym.Application.DTOs.Sales;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Controlador para gestionar ventas
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SalesController : BaseController
{
    public SalesController(ILogger<SalesController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    /// <summary>
    /// Crear venta manual
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale(
        [FromBody] CreateSaleDto createSaleDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();

            var command = new CreateSaleCommand
            {
                ClientId = createSaleDto.ClientId,
                ProductId = createSaleDto.ProductId,
                UserId = userId,
                FechaVenta = createSaleDto.FechaVenta,
                Pagado = createSaleDto.Pagado,
                MedioPago = createSaleDto.MedioPago,
                Referencia = createSaleDto.Referencia
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Venta creada: {SaleId}", result.Data);

            return CreatedResult(result.Data, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al crear venta");
            return InternalServerErrorResult("Error al crear la venta");
        }
    }

    /// <summary>
    /// Obtener historial de ventas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSalesHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? clientId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetSalesHistoryQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                ClientId = clientId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await Mediator.Send(query, cancellationToken);

            Logger.LogInformation("Historial de ventas obtenido: {Count} items", result.Data.Count);

            return OkResult(result, "Historial de ventas obtenido exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener historial de ventas");
            return InternalServerErrorResult("Error al obtener el historial de ventas");
        }
    }

    /// <summary>
    /// Obtener venta específica
    /// </summary>
    [HttpGet("{saleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSale(
        [FromRoute] Guid saleId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetSaleQuery { SaleId = saleId };

            var result = await Mediator.Send(query, cancellationToken);

            if (result is null)
                return NotFoundResult("Venta no encontrada");

            return OkResult(result, "Venta obtenida exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener venta {SaleId}", saleId);
            return InternalServerErrorResult("Error al obtener la venta");
        }
    }

    /// <summary>
    /// Marcar venta como pagada
    /// </summary>
    [HttpPut("{saleId}/mark-paid")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkSaleAsPaid(
        [FromRoute] Guid saleId,
        [FromBody] MarkSaleAsPaidDto markPaidDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new MarkSaleAsPaidCommand
            {
                SaleId = saleId,
                MedioPago = markPaidDto.MedioPago,
                Referencia = markPaidDto.Referencia
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Venta marcada como pagada: {SaleId}", saleId);

            return OkResult(null, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al marcar venta como pagada {SaleId}", saleId);
            return InternalServerErrorResult("Error al marcar la venta como pagada");
        }
    }
}
