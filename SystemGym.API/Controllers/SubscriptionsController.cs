namespace SystemGym.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using SystemGym.Application.Features.Subscriptions.Commands;
using SystemGym.Application.Features.Subscriptions.Queries;
using SystemGym.Application.DTOs.Subscriptions;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Controlador para gestionar suscripciones
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SubscriptionsController : BaseController
{
    public SubscriptionsController(ILogger<SubscriptionsController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    /// <summary>
    /// Crear una nueva suscripción
    /// Se genera automáticamente una venta asociada a la suscripción
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubscription(
        [FromBody] CreateSubscriptionDto createSubscriptionDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateSubscriptionCommand
            {
                ClientId = createSubscriptionDto.ClientId,
                PlanId = createSubscriptionDto.PlanId,
                InicioVigencia = createSubscriptionDto.InicioVigencia,
                TieneExpiracion = createSubscriptionDto.TieneExpiracion
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Suscripción creada: {SubscriptionId}", result.Data);

            return CreatedResult(result.Data, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al crear suscripción");
            return InternalServerErrorResult("Error al crear la suscripción");
        }
    }

    /// <summary>
    /// Obtener suscripciones de un cliente
    /// </summary>
    [HttpGet("client/{clientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientSubscriptions(
        [FromRoute] Guid clientId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetClientSubscriptionsQuery
            {
                ClientId = clientId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await Mediator.Send(query, cancellationToken);

            Logger.LogInformation(
                "Obteniendo suscripciones del cliente {ClientId}: página {PageNumber}",
                clientId,
                pageNumber);

            return OkResult(response, "Suscripciones obtenidas exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener suscripciones del cliente {ClientId}", clientId);
            return InternalServerErrorResult("Error al obtener las suscripciones");
        }
    }

    /// <summary>
    /// Renovar una suscripción existente.
    /// </summary>
    [HttpPut("{subscriptionId}/renew")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenewSubscription(
        [FromRoute] Guid subscriptionId,
        [FromBody] RenewSubscriptionDto renewSubscriptionDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new RenewSubscriptionCommand
            {
                SubscriptionId = subscriptionId,
                NuevoInicio = renewSubscriptionDto.NuevoInicio,
                TieneExpiracion = renewSubscriptionDto.TieneExpiracion
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success && result.Errors?.ContainsKey("SubscriptionId") == true)
                return NotFoundResult(result.Message ?? "Suscripción no encontrada");

            if (!result.Success)
                return BadRequestResult(result.Message ?? "No fue posible renovar la suscripción", result.Errors);

            return OkResult(new { subscriptionId = result.Data }, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al renovar suscripción {SubscriptionId}", subscriptionId);
            return InternalServerErrorResult("Error al renovar la suscripción");
        }
    }

    /// <summary>
    /// Cancelar una suscripción existente.
    /// </summary>
    [HttpDelete("{subscriptionId}")]
    [Authorize(Roles = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSubscription(
        [FromRoute] Guid subscriptionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelSubscriptionCommand
            {
                SubscriptionId = subscriptionId
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success && result.Errors?.ContainsKey("SubscriptionId") == true)
                return NotFoundResult(result.Message ?? "Suscripción no encontrada");

            if (!result.Success)
                return BadRequestResult(result.Message ?? "No fue posible cancelar la suscripción", result.Errors);

            return OkResult(new { subscriptionId = result.Data }, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al cancelar suscripción {SubscriptionId}", subscriptionId);
            return InternalServerErrorResult("Error al cancelar la suscripción");
        }
    }

    /// <summary>
    /// Obtener todas las suscripciones (ambos roles).
    /// Permite buscar por número de documento o por nombre/apellido del cliente.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscriptions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? tipoDocumento = null,
        [FromQuery] string? numeroDocumento = null,
        [FromQuery] string? nombreCliente = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetSubscriptionsQuery
            {
                PageNumber      = pageNumber,
                PageSize        = pageSize,
                TipoDocumento   = tipoDocumento,
                NumeroDocumento = numeroDocumento,
                NombreCliente   = nombreCliente
            };

            var result = await Mediator.Send(query, cancellationToken);

            if (!result.ClienteEncontrado)
            {
                var busqueda = !string.IsNullOrWhiteSpace(numeroDocumento)
                    ? $"documento {tipoDocumento} {numeroDocumento}"
                    : $"nombre '{nombreCliente}'";
                return NotFoundResult($"No existe ningún cliente con {busqueda}");
            }

            Logger.LogInformation("Suscripciones obtenidas: {Count} items", result.TotalCount);

            return OkResult(result, "Suscripciones obtenidas exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener suscripciones");
            return InternalServerErrorResult("Error al obtener las suscripciones");
        }
    }

    /// <summary>
    /// Registrar un ingreso en una suscripción.
    /// Solo 1 ingreso por día; descuenta de CantidadIngresos.
    /// </summary>
    [HttpPost("{subscriptionId}/ingreso")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterIngreso(
        [FromRoute] Guid subscriptionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new RegisterIngresoCommand
            {
                SubscriptionId = subscriptionId
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success && result.Errors?.ContainsKey("SubscriptionId") == true)
                return NotFoundResult(result.Message ?? "Suscripción no encontrada");

            if (!result.Success)
                return BadRequestResult(result.Message ?? "No fue posible registrar el ingreso", result.Errors);

            Logger.LogInformation("Ingreso registrado en suscripción {SubscriptionId}", subscriptionId);

            return OkResult(new { subscriptionId = result.Data }, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al registrar ingreso en suscripción {SubscriptionId}", subscriptionId);
            return InternalServerErrorResult("Error al registrar el ingreso");
        }
    }

    /// <summary>
    /// Exportar suscripciones de un cliente específico como archivo CSV.
    /// </summary>
    [HttpGet("client/{clientId}/export-csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportClientSubscriptionsCsv(
        [FromRoute] Guid clientId,
        CancellationToken cancellationToken)
    {
        var bytes = await Mediator.Send(
            new ExportClientSubscriptionsCsvQuery { ClientId = clientId },
            cancellationToken);

        var fileName = $"suscripciones-{clientId:N}-{DateTime.UtcNow:yyyyMMdd}.csv";
        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    /// <summary>
    /// Exportar resultados de búsqueda de suscripciones como archivo CSV.
    /// Acepta los mismos parámetros de búsqueda que GET /subscriptions.
    /// </summary>
    [HttpGet("export-csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportSubscriptionsCsv(
        [FromQuery] string? tipoDocumento   = null,
        [FromQuery] string? numeroDocumento = null,
        [FromQuery] string? nombreCliente   = null,
        CancellationToken cancellationToken = default)
    {
        var bytes = await Mediator.Send(new ExportSubscriptionsCsvQuery
        {
            TipoDocumento   = tipoDocumento,
            NumeroDocumento = numeroDocumento,
            NombreCliente   = nombreCliente
        }, cancellationToken);

        var fileName = $"suscripciones-busqueda-{DateTime.UtcNow:yyyyMMdd-HHmm}.csv";
        return File(bytes, "text/csv; charset=utf-8", fileName);
    }
}
