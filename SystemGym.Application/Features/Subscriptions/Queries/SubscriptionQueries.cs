namespace SystemGym.Application.Features.Subscriptions.Queries;

using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Subscriptions;

/// <summary>
/// Query para obtener suscripciones de un cliente
/// </summary>
public class GetClientSubscriptionsQuery : IQuery<SubscriptionsListResponseDto>
{
    public required Guid ClientId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? ActiveOnly { get; set; }
}

/// <summary>
/// Query para obtener todas las suscripciones, con búsqueda opcional por documento o nombre del cliente
/// </summary>
public class GetSubscriptionsQuery : IQuery<SubscriptionsListResponseDto>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    /// <summary>Búsqueda parcial por nombre o apellido del cliente</summary>
    public string? NombreCliente { get; set; }
}

/// <summary>
/// Query para obtener una suscripción específica
/// </summary>
public class GetSubscriptionQuery : IQuery<SubscriptionResponseDto?>
{
    public required Guid SubscriptionId { get; set; }
}
