namespace SystemGym.Application.Features.Clients.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para actualizar un cliente
/// </summary>
public class UpdateClientCommand : ICommand<UpdateClientCommandResult>
{
    public required Guid ClientId { get; set; }
    public required string TipoDocumento { get; set; }
    public required string NumeroDocumento { get; set; }
    public required string NombreCompleto { get; set; }
    public string? Celular { get; set; }
    public required bool Habilitado { get; set; }
}

/// <summary>
/// Resultado del comando de actualización de cliente
/// </summary>
public class UpdateClientCommandResult : CommandResult<Guid>
{
}
