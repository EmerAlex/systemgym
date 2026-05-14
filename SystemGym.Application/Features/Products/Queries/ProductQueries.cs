namespace SystemGym.Application.Features.Products.Queries;

using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Products;

/// <summary>
/// Query para obtener todos los productos
/// </summary>
public class GetProductsQuery : IQuery<ProductsListResponseDto>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Query para obtener un producto específico
/// </summary>
public class GetProductQuery : IQuery<ProductResponseDto?>
{
    public required Guid ProductId { get; set; }
}
