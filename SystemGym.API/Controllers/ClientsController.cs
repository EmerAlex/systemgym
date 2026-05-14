namespace SystemGym.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using SystemGym.Application.Features.Clients.Commands;
using SystemGym.Application.Features.Clients.Queries;
using SystemGym.Application.DTOs.Clients;

/// <summary>
/// Controlador para gestionar clientes
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ClientsController : BaseController
{
    public ClientsController(ILogger<ClientsController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    /// <summary>
    /// Crear un nuevo cliente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateClient(
        [FromBody] CreateClientDto createClientDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateClientCommand
            {
                TipoDocumento = createClientDto.TipoDocumento,
                NumeroDocumento = createClientDto.NumeroDocumento,
                NombreCompleto = createClientDto.NombreCompleto,
                Celular = createClientDto.Celular
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Cliente creado: {ClientId}", result.Data);

            return CreatedResult(result.Data, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al crear cliente");
            return InternalServerErrorResult("Error al crear el cliente");
        }
    }

    /// <summary>
    /// Obtener todos los clientes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllClients(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetClientsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            var response = await Mediator.Send(query, cancellationToken);

            Logger.LogInformation("Obteniendo clientes: página {PageNumber}, tamaño {PageSize}", pageNumber, pageSize);

            return OkResult(response, "Clientes obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener clientes");
            return InternalServerErrorResult("Error al obtener los clientes");
        }
    }

    /// <summary>
    /// Obtener cliente por ID
    /// </summary>
    [HttpGet("{clientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientById(
        [FromRoute] Guid clientId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetClientQuery
            {
                ClientId = clientId
            };

            var response = await Mediator.Send(query, cancellationToken);

            if (response is null)
                return NotFoundResult("Cliente no encontrado");

            return OkResult(response, "Cliente obtenido exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener cliente {ClientId}", clientId);
            return InternalServerErrorResult("Error al obtener el cliente");
        }
    }

    /// <summary>
    /// Actualizar cliente
    /// </summary>
    [HttpPut("{clientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClient(
        [FromRoute] Guid clientId,
        [FromBody] UpdateClientDto updateClientDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateClientCommand
            {
                ClientId = clientId,
                TipoDocumento = updateClientDto.TipoDocumento,
                NumeroDocumento = updateClientDto.NumeroDocumento,
                NombreCompleto = updateClientDto.NombreCompleto,
                Celular = updateClientDto.Celular,
                Habilitado = updateClientDto.Habilitado
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Cliente actualizado: {ClientId}", clientId);

            return OkResult(null, result.Message ?? "Cliente actualizado exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al actualizar cliente {ClientId}", clientId);
            return InternalServerErrorResult("Error al actualizar el cliente");
        }
    }
}
