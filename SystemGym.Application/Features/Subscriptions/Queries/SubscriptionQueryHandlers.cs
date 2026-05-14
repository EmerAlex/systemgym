namespace SystemGym.Application.Features.Subscriptions.Queries;

using AutoMapper;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Subscriptions;

/// <summary>
/// Utilidad compartida para enriquecer DTOs de suscripciones con la descripción del plan.
/// Evita duplicación entre los distintos query handlers.
/// </summary>
internal static class SubscriptionEnrichHelper
{
    internal static async Task EnrichWithPlanDescriptions(
        IEnumerable<SubscriptionResponseDto> dtos,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var planIds = dtos.Select(d => d.PlanId).Distinct().ToList();
        var planMap = new Dictionary<Guid, string>(planIds.Count);
        foreach (var planId in planIds)
        {
            var plan = await unitOfWork.Plans.GetByIdAsync(planId, cancellationToken);
            if (plan is not null)
                planMap[planId] = plan.Descripcion;
        }
        foreach (var dto in dtos)
            if (planMap.TryGetValue(dto.PlanId, out var desc))
                dto.PlanDescripcion = desc;
    }
}

/// <summary>
/// Handler para GetClientSubscriptionsQuery
/// </summary>
public class GetClientSubscriptionsQueryHandler : IQueryHandler<GetClientSubscriptionsQuery, SubscriptionsListResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetClientSubscriptionsQueryHandler> _logger;

    public GetClientSubscriptionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetClientSubscriptionsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SubscriptionsListResponseDto> Handle(GetClientSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (items, total) = await _unitOfWork.Subscriptions
                .GetPagedByClientAsync(request.ClientId, request.PageNumber, request.PageSize, request.ActiveOnly, cancellationToken);

            // Evaluar y auto-desactivar suscripciones vencidas o sin ingresos
            var itemsList = items.ToList();
            var anyChanged = false;
            foreach (var sub in itemsList)
            {
                if (sub.EvaluateAndDeactivateIfNeeded())
                {
                    _unitOfWork.Subscriptions.Update(sub);
                    anyChanged = true;
                }
            }
            if (anyChanged)
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            var subDtos = _mapper.Map<List<SubscriptionResponseDto>>(itemsList);

            // Enriquecer con información del cliente
            var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId, cancellationToken);
            if (client is not null)
            {
                foreach (var dto in subDtos)
                {
                    dto.ClientNombreCompleto = client.NombreCompleto;
                    dto.ClientTipoDocumento = client.TipoDocumento.Value;
                    dto.ClientNumeroDocumento = client.NumeroDocumento;
                }
            }

            await SubscriptionEnrichHelper.EnrichWithPlanDescriptions(subDtos, _unitOfWork, cancellationToken);

            _logger.LogInformation("Suscripciones del cliente {ClientId} obtenidas: {Count} items (página {PageNumber})",
                request.ClientId, subDtos.Count, request.PageNumber);

            return new SubscriptionsListResponseDto
            {
                Data = subDtos,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(total / (double)request.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener suscripciones del cliente {ClientId}", request.ClientId);
            throw;
        }
    }
}

/// <summary>
/// Handler para GetSubscriptionsQuery
/// </summary>
public class GetSubscriptionsQueryHandler : IQueryHandler<GetSubscriptionsQuery, SubscriptionsListResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSubscriptionsQueryHandler> _logger;

    public GetSubscriptionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSubscriptionsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SubscriptionsListResponseDto> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo suscripciones: página {PageNumber}, búsqueda: {TipoDoc} {NumDoc} {Nombre}",
                request.PageNumber, request.TipoDocumento, request.NumeroDocumento, request.NombreCliente);

            // Sin parámetros de búsqueda → devolver todas las suscripciones paginadas
            var tieneDocumento = !string.IsNullOrWhiteSpace(request.NumeroDocumento);
            var tieneNombre    = !string.IsNullOrWhiteSpace(request.NombreCliente);

            if (!tieneDocumento && !tieneNombre)
            {
                var (allItems, allTotal) = await _unitOfWork.Subscriptions
                    .GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

                var allItemsList = allItems.ToList();
                var anyChangedAll = false;
                foreach (var sub in allItemsList)
                {
                    if (sub.EvaluateAndDeactivateIfNeeded())
                    {
                        _unitOfWork.Subscriptions.Update(sub);
                        anyChangedAll = true;
                    }
                }
                if (anyChangedAll)
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                var allDtos = _mapper.Map<List<SubscriptionResponseDto>>(allItemsList);

                // Enriquecer con información del cliente para cada suscripción
                var clientCache = new Dictionary<Guid, Domain.Entities.Client>();
                foreach (var dto in allDtos)
                {
                    if (!clientCache.TryGetValue(dto.ClientId, out var cachedClient))
                    {
                        cachedClient = await _unitOfWork.Clients.GetByIdAsync(dto.ClientId, cancellationToken);
                        clientCache[dto.ClientId] = cachedClient;
                    }

                    if (cachedClient is not null)
                    {
                        dto.ClientNombreCompleto = cachedClient.NombreCompleto;
                        dto.ClientTipoDocumento = cachedClient.TipoDocumento.Value;
                        dto.ClientNumeroDocumento = cachedClient.NumeroDocumento;
                    }
                }

                await SubscriptionEnrichHelper.EnrichWithPlanDescriptions(allDtos, _unitOfWork, cancellationToken);

                return new SubscriptionsListResponseDto
                {
                    Data = allDtos,
                    TotalCount = allTotal,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling(allTotal / (double)request.PageSize),
                    ClienteEncontrado = true
                };
            }

            // ──────────────────────────────────────────────
            // CASO A: Búsqueda por número de documento
            // ──────────────────────────────────────────────
            if (tieneDocumento)
            {
                var client = await _unitOfWork.Clients.GetByNumeroDocumentoAsync(
                    request.NumeroDocumento!.Trim(),
                    request.TipoDocumento,
                    cancellationToken);

                if (client is null)
                {
                    _logger.LogInformation("Cliente no encontrado: {TipoDoc} {NumDoc}", request.TipoDocumento, request.NumeroDocumento);
                    return new SubscriptionsListResponseDto
                    {
                        Data = [],
                        TotalCount = 0,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalPages = 0,
                        ClienteEncontrado = false
                    };
                }

                return await BuildResponseForSingleClient(client, request, cancellationToken);
            }

            // ──────────────────────────────────────────────
            // CASO B: Búsqueda por nombre (puede devolver varios clientes)
            // ──────────────────────────────────────────────
            var clientes = (await _unitOfWork.Clients.SearchByNombreAsync(request.NombreCliente!.Trim(), cancellationToken)).ToList();

            if (clientes.Count == 0)
            {
                _logger.LogInformation("Ningún cliente encontrado con nombre: {Nombre}", request.NombreCliente);
                return new SubscriptionsListResponseDto
                {
                    Data = [],
                    TotalCount = 0,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = 0,
                    ClienteEncontrado = false
                };
            }

            // Agregar suscripciones de todos los clientes que coincidan
            var todasSubs = new List<SubscriptionResponseDto>();
            foreach (var c in clientes)
            {
                var (items, _) = await _unitOfWork.Subscriptions
                    .GetPagedByClientAsync(c.Id, 1, 100, null, cancellationToken);

                var dtos = _mapper.Map<List<SubscriptionResponseDto>>(items);
                foreach (var dto in dtos)
                {
                    dto.ClientNombreCompleto = c.NombreCompleto;
                    dto.ClientTipoDocumento  = c.TipoDocumento.Value;
                    dto.ClientNumeroDocumento = c.NumeroDocumento;
                }
                todasSubs.AddRange(dtos);
            }

            // Enriquecer con descripción del plan
            await SubscriptionEnrichHelper.EnrichWithPlanDescriptions(todasSubs, _unitOfWork, cancellationToken);

            // Paginación manual sobre el resultado agregado
            var totalCount  = todasSubs.Count;
            var paginado    = todasSubs
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new SubscriptionsListResponseDto
            {
                Data          = paginado,
                TotalCount    = totalCount,
                PageNumber    = request.PageNumber,
                PageSize      = request.PageSize,
                TotalPages    = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                ClienteEncontrado = true,
                ClienteNombreCompleto = clientes.Count == 1 ? clientes[0].NombreCompleto : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener suscripciones");
            throw;
        }
    }

    private async Task<SubscriptionsListResponseDto> BuildResponseForSingleClient(
        SystemGym.Domain.Entities.Client client,
        GetSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await _unitOfWork.Subscriptions
            .GetPagedByClientAsync(client.Id, request.PageNumber, request.PageSize, null, cancellationToken);

        // Evaluar y auto-desactivar
        var itemsList = items.ToList();
        var anyChanged = false;
        foreach (var sub in itemsList)
        {
            if (sub.EvaluateAndDeactivateIfNeeded())
            {
                _unitOfWork.Subscriptions.Update(sub);
                anyChanged = true;
            }
        }
        if (anyChanged)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        var subDtos = _mapper.Map<List<SubscriptionResponseDto>>(itemsList);

        var clientNombre   = client.NombreCompleto;
        var clientTipoDoc  = client.TipoDocumento.Value;
        var clientNumDoc   = client.NumeroDocumento;

        // Enriquecer con información del cliente
        foreach (var dto in subDtos)
        {
            dto.ClientNombreCompleto = clientNombre;
            dto.ClientTipoDocumento = clientTipoDoc;
            dto.ClientNumeroDocumento = clientNumDoc;
        }

        await SubscriptionEnrichHelper.EnrichWithPlanDescriptions(subDtos, _unitOfWork, cancellationToken);

        return new SubscriptionsListResponseDto
        {
            Data          = subDtos,
            TotalCount    = total,
            PageNumber    = request.PageNumber,
            PageSize      = request.PageSize,
            TotalPages    = (int)Math.Ceiling(total / (double)request.PageSize),
            ClienteEncontrado      = true,
            ClienteNombreCompleto  = clientNombre
        };
    }

}

/// <summary>
/// Handler para GetSubscriptionQuery
/// </summary>
public class GetSubscriptionQueryHandler : IQueryHandler<GetSubscriptionQuery, SubscriptionResponseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSubscriptionQueryHandler> _logger;

    public GetSubscriptionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSubscriptionQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SubscriptionResponseDto?> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo suscripción {SubscriptionId}", request.SubscriptionId);

            var subscription = await _unitOfWork.Subscriptions
                .GetByIdAsync(request.SubscriptionId, cancellationToken);

            if (subscription is null)
            {
                _logger.LogWarning("Suscripción no encontrada: {SubscriptionId}", request.SubscriptionId);
                return null;
            }

            return _mapper.Map<SubscriptionResponseDto>(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener suscripción {SubscriptionId}", request.SubscriptionId);
            throw;
        }
    }
}
