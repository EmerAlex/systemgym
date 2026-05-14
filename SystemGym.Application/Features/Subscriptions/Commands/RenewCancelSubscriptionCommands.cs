namespace SystemGym.Application.Features.Subscriptions.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para renovar una suscripción.
/// </summary>
public class RenewSubscriptionCommand : ICommand<RenewSubscriptionCommandResult>
{
    public required Guid SubscriptionId { get; set; }
    public required DateTime NuevoInicio { get; set; }
    public bool TieneExpiracion { get; set; } = true;
}

public class RenewSubscriptionCommandResult : CommandResult<Guid>
{
}

/// <summary>
/// Comando para cancelar una suscripción.
/// </summary>
public class CancelSubscriptionCommand : ICommand<CancelSubscriptionCommandResult>
{
    public required Guid SubscriptionId { get; set; }
}

public class CancelSubscriptionCommandResult : CommandResult<Guid>
{
}
