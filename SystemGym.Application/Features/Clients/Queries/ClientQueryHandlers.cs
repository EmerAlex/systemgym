namespace SystemGym.Application.Features.Clients.Queries;

using AutoMapper;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Clients;

/// <summary>
/// Handler para GetClientsQuery - Obtiene listado paginado de clientes
/// </summary>
public class GetClientsQueryHandler : IQueryHandler<GetClientsQuery, ClientsListResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetClientsQueryHandler> _logger;

    public GetClientsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetClientsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ClientsListResponseDto> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo clientes: página {PageNumber}, tamaño {PageSize}", request.PageNumber, request.PageSize);

            var (items, total) = await _unitOfWork.Clients
                .GetPagedAsync(request.PageNumber, request.PageSize, request.SearchTerm, cancellationToken);

            var clientDtos = _mapper.Map<List<ClientResponseDto>>(items);

            return new ClientsListResponseDto
            {
                Data = clientDtos,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes");
            throw;
        }
    }
}

/// <summary>
/// Handler para GetClientQuery - Obtiene un cliente específico
/// </summary>
public class GetClientQueryHandler : IQueryHandler<GetClientQuery, ClientResponseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetClientQueryHandler> _logger;

    public GetClientQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetClientQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ClientResponseDto?> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo cliente {ClientId}", request.ClientId);

            var client = await _unitOfWork.Clients
                .GetByIdAsync(request.ClientId, cancellationToken);

            if (client is null)
            {
                _logger.LogWarning("Cliente no encontrado: {ClientId}", request.ClientId);
                return null;
            }

            return _mapper.Map<ClientResponseDto>(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente {ClientId}", request.ClientId);
            throw;
        }
    }
}
