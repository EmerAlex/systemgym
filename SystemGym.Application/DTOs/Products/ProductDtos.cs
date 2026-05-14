namespace SystemGym.Application.DTOs.Products;

using SystemGym.Application.DTOs.Common;

/// <summary>
/// DTO para crear producto
/// </summary>
public class CreateProductDto
{
    public string Descripcion { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

/// <summary>
/// DTO de respuesta para producto
/// </summary>
public class ProductResponseDto
{
    public Guid ProductId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public bool Habilitado { get; set; }
    public int CantidadActual { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para actualizar producto
/// </summary>
public class UpdateProductDto
{
    public string Descripcion { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

/// <summary>
/// DTO para respuesta de listado de productos
/// </summary>
public class ProductsListResponseDto : PaginatedResponseDto<ProductResponseDto>
{
}
