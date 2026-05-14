namespace SystemGym.Application.DTOs.Sales;

using SystemGym.Application.DTOs.Common;

/// <summary>
/// DTO para crear venta
/// </summary>
public class CreateSaleDto
{
    public Guid ClientId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime FechaVenta { get; set; }
    public bool Pagado { get; set; }
    public string? MedioPago { get; set; }
    public string? Referencia { get; set; }
}

/// <summary>
/// DTO de respuesta para venta
/// </summary>
public class SaleResponseDto
{
    public Guid SaleId { get; set; }
    public Guid ClientId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductoDescripcion { get; set; } = string.Empty;
    public Guid? SubscriptionId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime FechaVenta { get; set; }
    public decimal Monto { get; set; }
    public bool Pagado { get; set; }
    public string? MedioPago { get; set; }
    public string? Referencia { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para marcar venta como pagada
/// </summary>
public class MarkSaleAsPaidDto
{
    public string? MedioPago { get; set; }
    public string? Referencia { get; set; }
}

/// <summary>
/// DTO para respuesta de listado de ventas
/// </summary>
public class SalesListResponseDto : PaginatedResponseDto<SaleResponseDto>
{
    public decimal Total { get; set; }
}
