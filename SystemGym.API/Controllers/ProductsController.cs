namespace SystemGym.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using SystemGym.Application.Features.Products.Commands;
using SystemGym.Application.Features.Products.Queries;
using SystemGym.Application.DTOs.Products;

/// <summary>
/// Controlador para gestionar productos
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProductsController : BaseController
{
    public ProductsController(ILogger<ProductsController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    /// <summary>
    /// Crear nuevo producto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductDto createProductDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateProductCommand
            {
                Descripcion = createProductDto.Descripcion,
                Valor = createProductDto.Valor
            };

            var result = await Mediator.Send(command, cancellationToken);

            if (!result.Success)
                return BadRequestResult(result.Message, result.Errors);

            Logger.LogInformation("Producto creado: {ProductId}", result.Data);

            return CreatedResult(result.Data, result.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al crear producto");
            return InternalServerErrorResult("Error al crear el producto");
        }
    }

    /// <summary>
    /// Obtener todos los productos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetProductsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await Mediator.Send(query, cancellationToken);

            Logger.LogInformation("Productos obtenidos: {Count} items", result.Data.Count);

            return OkResult(result, "Productos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener productos");
            return InternalServerErrorResult("Error al obtener los productos");
        }
    }

    /// <summary>
    /// Obtener producto por ID
    /// </summary>
    [HttpGet("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetProductQuery { ProductId = productId };

            var result = await Mediator.Send(query, cancellationToken);

            if (result is null)
                return NotFoundResult("Producto no encontrado");

            return OkResult(result, "Producto obtenido exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener producto {ProductId}", productId);
            return InternalServerErrorResult("Error al obtener el producto");
        }
    }
}
