namespace SystemGym.Application.Features.Sales.Queries;

using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Sales;

/// <summary>
/// Query para obtener historial de ventas
/// </summary>
public class GetSalesHistoryQuery : IQuery<SalesListResponseDto>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? ClientId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

/// <summary>
/// Query para obtener venta específica
/// </summary>
public class GetSaleQuery : IQuery<SaleResponseDto?>
{
    public required Guid SaleId { get; set; }
}
