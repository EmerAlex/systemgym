namespace SystemGym.Application.Features.Clients.Queries;

using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Clients;

/// <summary>
/// Query para obtener todos los clientes con paginación
/// </summary>
public class GetClientsQuery : IQuery<ClientsListResponseDto>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

/// <summary>
/// Query para obtener un cliente específico por ID
/// </summary>
public class GetClientQuery : IQuery<ClientResponseDto?>
{
    public required Guid ClientId { get; set; }
}
