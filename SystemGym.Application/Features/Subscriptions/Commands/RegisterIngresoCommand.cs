namespace SystemGym.Application.Features.Subscriptions.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para registrar un ingreso en una suscripción.
/// Descuenta un ingreso y valida que no se haya registrado otro hoy.
/// </summary>
public class RegisterIngresoCommand : ICommand<RegisterIngresoCommandResult>
{
    public required Guid SubscriptionId { get; set; }
}

/// <summary>
/// Resultado del comando de registro de ingreso
/// </summary>
public class RegisterIngresoCommandResult : CommandResult<Guid>
{
}
