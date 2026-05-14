namespace SystemGym.Application.Features.Plans.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para crear un plan
/// </summary>
public class CreatePlanCommand : ICommand<CreatePlanCommandResult>
{
    public required string Descripcion { get; set; }
    public required string TipoPeriodo { get; set; }
    public required int CantidadIntervalosPeriodo { get; set; }
    public required decimal Valor { get; set; }
}

/// <summary>
/// Resultado del comando de creación de plan
/// </summary>
public class CreatePlanCommandResult : CommandResult<Guid>
{
}
