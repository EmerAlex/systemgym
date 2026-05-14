namespace SystemGym.Application.Features.Subscriptions.Queries;

using SystemGym.Application.Abstractions;

/// <summary>
/// Query para exportar suscripciones de un cliente como CSV
/// </summary>
public class ExportClientSubscriptionsCsvQuery : IQuery<byte[]>
{
    public required Guid ClientId { get; set; }
}

/// <summary>
/// Query para exportar el resultado de una búsqueda global como CSV
/// </summary>
public class ExportSubscriptionsCsvQuery : IQuery<byte[]>
{
    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? NombreCliente { get; set; }
}
