namespace SystemGym.Application.DTOs.Clients;

using SystemGym.Application.DTOs.Common;

/// <summary>
/// DTO para crear cliente
/// </summary>
public class CreateClientDto
{
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Celular { get; set; }
}

/// <summary>
/// DTO de respuesta para cliente
/// </summary>
public class ClientResponseDto
{
    public Guid ClientId { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Celular { get; set; }
    public bool Habilitado { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para actualizar datos de cliente
/// </summary>
public class UpdateClientDto
{
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Celular { get; set; }
    public bool Habilitado { get; set; }
}

/// <summary>
/// DTO para respuesta de listado de clientes
/// </summary>
public class ClientsListResponseDto : PaginatedResponseDto<ClientResponseDto>
{
}
