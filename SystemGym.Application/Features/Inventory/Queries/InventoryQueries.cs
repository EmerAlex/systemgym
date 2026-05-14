namespace SystemGym.Application.Features.Inventory.Queries;

using SystemGym.Application.Abstractions;

/// <summary>
/// DTO para log de inventario
/// </summary>
public class InventoryLogDto
{
    public Guid LogId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductoDescripcion { get; set; } = string.Empty;
    public int CantidadAnterior { get; set; }
    public int CantidadNueva { get; set; }
    public int Diferencia { get; set; }
    public string Operacion { get; set; } = string.Empty;
    public string MotivoAuditoria { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para respuesta de logs de inventario
/// </summary>
public class InventoryLogsListResponseDto
{
    public List<InventoryLogDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Query para obtener logs de inventario
/// </summary>
public class GetInventoryLogsQuery : IQuery<InventoryLogsListResponseDto>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? ProductId { get; set; }
}

/// <summary>
/// Query para obtener logs de un producto
/// </summary>
public class GetProductInventoryLogsQuery : IQuery<InventoryLogsListResponseDto>
{
    public required Guid ProductId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
