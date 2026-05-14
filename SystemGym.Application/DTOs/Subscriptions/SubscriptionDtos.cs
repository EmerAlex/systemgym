namespace SystemGym.Application.DTOs.Subscriptions;

using SystemGym.Application.DTOs.Common;

/// <summary>
/// DTO para crear suscripción
/// </summary>
public class CreateSubscriptionDto
{
    public Guid ClientId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime InicioVigencia { get; set; }
    public bool TieneExpiracion { get; set; } = true;
}

/// <summary>
/// DTO de respuesta para suscripción
/// </summary>
public class SubscriptionResponseDto
{
    public Guid SubscriptionId { get; set; }
    public Guid ClientId { get; set; }
    public string? ClientNombreCompleto { get; set; }
    public string? ClientTipoDocumento { get; set; }
    public string? ClientNumeroDocumento { get; set; }
    public Guid PlanId { get; set; }
    public string PlanDescripcion { get; set; } = string.Empty;
    public DateTime InicioVigencia { get; set; }
    public DateTime? FinVigencia { get; set; }  // Nullable si TieneExpiracion es false
    public bool TieneExpiracion { get; set; }
    public bool Activa { get; set; }
    public DateTime? UltimoIngreso { get; set; }
    public int CantidadIngresos { get; set; }
    public decimal Valor { get; set; }
    public SaleGeneratedDto? SaleGenerated { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para venta generada automáticamente
/// </summary>
public class SaleGeneratedDto
{
    public Guid SaleId { get; set; }
    public decimal Monto { get; set; }
    public bool Pagado { get; set; }
    public DateTime FechaVenta { get; set; }
}

/// <summary>
/// DTO para renovar suscripción
/// </summary>
public class RenewSubscriptionDto
{
    public DateTime NuevoInicio { get; set; }
    public bool TieneExpiracion { get; set; } = true;
}

/// <summary>
/// DTO para respuesta de listado de suscripciones
/// </summary>
public class SubscriptionsListResponseDto : PaginatedResponseDto<SubscriptionResponseDto>
{
    /// <summary>
    /// Indica si el cliente fue encontrado en la búsqueda por documento
    /// </summary>
    public bool ClienteEncontrado { get; set; } = true;

    /// <summary>
    /// Nombre completo del cliente encontrado (para mostrar en la cabecera de resultados)
    /// </summary>
    public string? ClienteNombreCompleto { get; set; }
}
