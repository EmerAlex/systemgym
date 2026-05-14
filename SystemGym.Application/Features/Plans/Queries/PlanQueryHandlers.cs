namespace SystemGym.Application.Features.Plans.Queries;

using AutoMapper;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Plans;

/// <summary>
/// Handler para GetPlansQuery
/// </summary>
public class GetPlansQueryHandler : IQueryHandler<GetPlansQuery, PlansListResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPlansQueryHandler> _logger;

    public GetPlansQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPlansQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PlansListResponseDto> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo planes: página {PageNumber}", request.PageNumber);

            var (items, total) = await _unitOfWork.Plans
                .GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            var planDtos = _mapper.Map<List<PlanResponseDto>>(items);

            return new PlansListResponseDto
            {
                Data = planDtos,
                TotalCount = total
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener planes");
            throw;
        }
    }
}

/// <summary>
/// Handler para GetPlanQuery
/// </summary>
public class GetPlanQueryHandler : IQueryHandler<GetPlanQuery, PlanResponseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPlanQueryHandler> _logger;

    public GetPlanQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPlanQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PlanResponseDto?> Handle(GetPlanQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Obteniendo plan {PlanId}", request.PlanId);

            var plan = await _unitOfWork.Plans
                .GetByIdAsync(request.PlanId, cancellationToken);

            if (plan is null)
            {
                _logger.LogWarning("Plan no encontrado: {PlanId}", request.PlanId);
                return null;
            }

            return _mapper.Map<PlanResponseDto>(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plan {PlanId}", request.PlanId);
            throw;
        }
    }
}
