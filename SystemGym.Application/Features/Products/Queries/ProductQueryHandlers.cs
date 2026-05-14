namespace SystemGym.Application.Features.Products.Queries;

using AutoMapper;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Products;

/// <summary>
/// Handler para GetProductsQuery
/// </summary>
public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, ProductsListResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetProductsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductsListResponseDto> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo productos: página {PageNumber}", request.PageNumber);

            var (items, total) = await _unitOfWork.Products
                .GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            var productDtos = _mapper.Map<List<ProductResponseDto>>(items);

            return new ProductsListResponseDto
            {
                Data = productDtos,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            throw;
        }
    }
}

/// <summary>
/// Handler para GetProductQuery
/// </summary>
public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductResponseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductQueryHandler> _logger;

    public GetProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetProductQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductResponseDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo producto {ProductId}", request.ProductId);

            var product = await _unitOfWork.Products
                .GetByIdAsync(request.ProductId, cancellationToken);

            if (product is null)
            {
                _logger.LogWarning("Producto no encontrado: {ProductId}", request.ProductId);
                return null;
            }

            return _mapper.Map<ProductResponseDto>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto {ProductId}", request.ProductId);
            throw;
        }
    }
}
