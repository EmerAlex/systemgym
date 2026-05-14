namespace SystemGym.Application.Features.Plans.Queries;

using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Plans;

/// <summary>
/// Query para obtener todos los planes
/// </summary>
public class GetPlansQuery : IQuery<PlansListResponseDto>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Query para obtener un plan específico
/// </summary>
public class GetPlanQuery : IQuery<PlanResponseDto?>
{
    public required Guid PlanId { get; set; }
}
