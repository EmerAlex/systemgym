namespace SystemGym.Application.Features.Plans.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para eliminar un plan
/// </summary>
public class DeletePlanCommand : ICommand<DeletePlanCommandResult>
{
    public required Guid PlanId { get; set; }
}

/// <summary>
/// Resultado del comando de eliminación de plan
/// </summary>
public class DeletePlanCommandResult : CommandResult<Guid>
{
}
