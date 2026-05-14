namespace SystemGym.Application.Features.Inventory.Queries;

using AutoMapper;
using SystemGym.Application.Abstractions;

/// <summary>
/// Handler para GetInventoryLogsQuery
/// </summary>
public class GetInventoryLogsQueryHandler : IQueryHandler<GetInventoryLogsQuery, InventoryLogsListResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInventoryLogsQueryHandler> _logger;

    public GetInventoryLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetInventoryLogsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InventoryLogsListResponseDto> Handle(GetInventoryLogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo logs de inventario: pagina {PageNumber}", request.PageNumber);

            var (items, total) = await _unitOfWork.InventoryLogs
                .GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            var logs = items.AsEnumerable();

            if (request.ProductId.HasValue)
                logs = logs.Where(l => l.ProductId == request.ProductId.Value);

            var logsList = logs.ToList();

            // Resolver descripcion de producto para cada log
            var productIds = logsList.Select(l => l.ProductId).Distinct().ToList();
            var products = new Dictionary<Guid, string>();
            foreach (var pid in productIds)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(pid, cancellationToken);
                if (product != null)
                    products[pid] = product.Descripcion;
            }

            var logDtos = _mapper.Map<List<InventoryLogDto>>(logsList);
            foreach (var dto in logDtos)
                dto.ProductoDescripcion = products.TryGetValue(dto.ProductId, out var desc) ? desc : string.Empty;

            return new InventoryLogsListResponseDto
            {
                Data = logDtos,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener logs de inventario");
            throw;
        }
    }
}

/// <summary>
/// Handler para GetProductInventoryLogsQuery
/// </summary>
public class GetProductInventoryLogsQueryHandler : IQueryHandler<GetProductInventoryLogsQuery, InventoryLogsListResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductInventoryLogsQueryHandler> _logger;

    public GetProductInventoryLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetProductInventoryLogsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InventoryLogsListResponseDto> Handle(GetProductInventoryLogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo logs de inventario del producto {ProductId}", request.ProductId);

            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
            var productoDesc = product?.Descripcion ?? string.Empty;

            var logs = await _unitOfWork.InventoryLogs
                .GetByProductIdAsync(request.ProductId, cancellationToken);

            var logsList = logs
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var logDtos = _mapper.Map<List<InventoryLogDto>>(logsList);
            foreach (var dto in logDtos)
                dto.ProductoDescripcion = productoDesc;

            return new InventoryLogsListResponseDto
            {
                Data = logDtos,
                TotalCount = logs.Count(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(logs.Count() / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener logs de inventario del producto {ProductId}", request.ProductId);
            throw;
        }
    }
}
