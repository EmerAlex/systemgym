namespace SystemGym.Application.Features.Subscriptions.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para crear una suscripción
/// </summary>
public class CreateSubscriptionCommand : ICommand<CreateSubscriptionCommandResult>
{
    public required Guid ClientId { get; set; }
    public required Guid PlanId { get; set; }
    public required DateTime InicioVigencia { get; set; }
    public bool TieneExpiracion { get; set; } = true;  // Si es false, FinVigencia será null
}

/// <summary>
/// Resultado del comando de creación de suscripción
/// </summary>
public class CreateSubscriptionCommandResult : CommandResult<Guid>
{
}
