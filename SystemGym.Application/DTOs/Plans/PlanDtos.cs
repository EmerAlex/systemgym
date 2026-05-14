namespace SystemGym.Application.DTOs.Plans;

using SystemGym.Application.DTOs.Common;

/// <summary>
/// DTO para crear plan
/// </summary>
public class CreatePlanDto
{
    public string Descripcion { get; set; } = string.Empty;
    public string TipoPeriodo { get; set; } = "Mes";
    public int CantidadIntervalosPeriodo { get; set; } = 1;
    public decimal Valor { get; set; }
}

/// <summary>
/// DTO de respuesta para plan
/// </summary>
public class PlanResponseDto
{
    public Guid PlanId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string TipoPeriodo { get; set; } = string.Empty;
    public int CantidadIntervalosPeriodo { get; set; }
    public decimal Valor { get; set; }
    public bool Habilitado { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO para actualizar plan
/// </summary>
public class UpdatePlanDto
{
    public string Descripcion { get; set; } = string.Empty;
    public string TipoPeriodo { get; set; } = "Mes";
    public int CantidadIntervalosPeriodo { get; set; } = 1;
    public decimal Valor { get; set; }
}

/// <summary>
/// DTO para respuesta de listado de planes
/// </summary>
public class PlansListResponseDto : PaginatedResponseDto<PlanResponseDto>
{
}
