namespace SystemGym.Application.Features.Sales.Queries;

using AutoMapper;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Sales;

/// <summary>
/// Handler para GetSalesHistoryQuery
/// </summary>
public class GetSalesHistoryQueryHandler : IQueryHandler<GetSalesHistoryQuery, SalesListResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSalesHistoryQueryHandler> _logger;

    public GetSalesHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSalesHistoryQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SalesListResponseDto> Handle(GetSalesHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo historial de ventas: página {PageNumber}", request.PageNumber);

            var (items, total) = await _unitOfWork.SalesHistory
                .GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            // Filtrar por cliente si se proporciona
            var sales = items.AsEnumerable();
            
            if (request.ClientId.HasValue)
                sales = sales.Where(s => s.ClientId == request.ClientId.Value);

            // Filtrar por rango de fechas si se proporciona
            if (request.FromDate.HasValue)
                sales = sales.Where(s => s.FechaVenta >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                sales = sales.Where(s => s.FechaVenta <= request.ToDate.Value);

            var salesList = sales.ToList();
            var saleDtos = _mapper.Map<List<SaleResponseDto>>(salesList);

            await EnrichSalesAsync(salesList, saleDtos, cancellationToken);

            return new SalesListResponseDto
            {
                Data = saleDtos,
                Total = saleDtos.Sum(s => s.Monto),
                TotalCount = salesList.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(salesList.Count / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de ventas");
            throw;
        }
    }

    private async Task EnrichSalesAsync(
        List<Domain.Entities.SalesHistory> salesList,
        List<SaleResponseDto> saleDtos,
        CancellationToken cancellationToken)
    {
        var clientNames = new Dictionary<Guid, string>();
        var productDescriptions = new Dictionary<Guid, string>();
        var planDescriptions = new Dictionary<Guid, string>();
        var userNames = new Dictionary<Guid, string>();

        // Cargar nombres de clientes
        foreach (var clientId in salesList.Select(s => s.ClientId).Distinct())
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId, cancellationToken);
            if (client is not null)
                clientNames[clientId] = client.NombreCompleto;
        }

        // Cargar descripciones de productos
        foreach (var productId in salesList.Where(s => !s.SubscriptionId.HasValue).Select(s => s.ProductId).Distinct())
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product is not null)
                productDescriptions[productId] = product.Descripcion;
        }

        // Cargar descripciones de planes
        foreach (var planId in salesList.Where(s => s.SubscriptionId.HasValue).Select(s => s.ProductId).Distinct())
        {
            var plan = await _unitOfWork.Plans.GetByIdAsync(planId, cancellationToken);
            if (plan is not null)
                planDescriptions[planId] = plan.Descripcion;
        }

        // Cargar nombres de usuarios
        foreach (var userId in salesList.Select(s => s.UserId).Where(u => u.HasValue).Distinct())
        {
            if (userId.HasValue)
            {
                var user = await _unitOfWork.SystemUsers.GetByIdAsync(userId.Value, cancellationToken);
                if (user is not null)
                    userNames[userId.Value] = user.Username;
            }
        }

        // Enriquecer DTOs
        foreach (var dto in saleDtos)
        {
            dto.ClienteNombre = clientNames.TryGetValue(dto.ClientId, out var clientName)
                ? clientName
                : string.Empty;

            dto.ProductoDescripcion = dto.SubscriptionId.HasValue
                ? (planDescriptions.TryGetValue(dto.ProductId, out var planDescription) ? planDescription : string.Empty)
                : (productDescriptions.TryGetValue(dto.ProductId, out var productDescription) ? productDescription : string.Empty);

            dto.UserName = userNames.TryGetValue(dto.UserId, out var userName)
                ? userName
                : "Desconocido";
        }
    }
}

/// <summary>
/// Handler para GetSaleQuery
/// </summary>
public class GetSaleQueryHandler : IQueryHandler<GetSaleQuery, SaleResponseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleQueryHandler> _logger;

    public GetSaleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSaleQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResponseDto?> Handle(GetSaleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo venta {SaleId}", request.SaleId);

            var sale = await _unitOfWork.SalesHistory
                .GetByIdAsync(request.SaleId, cancellationToken);

            if (sale is null)
            {
                _logger.LogWarning("Venta no encontrada: {SaleId}", request.SaleId);
                return null;
            }

            var dto = _mapper.Map<SaleResponseDto>(sale);

            var client = await _unitOfWork.Clients.GetByIdAsync(sale.ClientId, cancellationToken);
            dto.ClienteNombre = client?.NombreCompleto ?? string.Empty;

            if (sale.UserId.HasValue)
            {
                var user = await _unitOfWork.SystemUsers.GetByIdAsync(sale.UserId.Value, cancellationToken);
                dto.UserName = user?.Username ?? "Desconocido";
            }
            else
            {
                dto.UserName = "Sistema"; // Para ventas generadas automáticamente
            }

            if (sale.SubscriptionId.HasValue)
            {
                var plan = await _unitOfWork.Plans.GetByIdAsync(sale.ProductId, cancellationToken);
                dto.ProductoDescripcion = plan?.Descripcion ?? string.Empty;
            }
            else
            {
                var product = await _unitOfWork.Products.GetByIdAsync(sale.ProductId, cancellationToken);
                dto.ProductoDescripcion = product?.Descripcion ?? string.Empty;
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener venta {SaleId}", request.SaleId);
            throw;
        }
    }
}
