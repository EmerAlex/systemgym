namespace SystemGym.Application.Features.Plans.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para actualizar un plan
/// </summary>
public class UpdatePlanCommand : ICommand<UpdatePlanCommandResult>
{
    public required Guid PlanId { get; set; }
    public required string Descripcion { get; set; }
    public required string TipoPeriodo { get; set; }
    public required int CantidadIntervalosPeriodo { get; set; }
    public required decimal Valor { get; set; }
}

/// <summary>
/// Resultado del comando de actualización de plan
/// </summary>
public class UpdatePlanCommandResult : CommandResult<Guid>
{
}
