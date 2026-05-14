namespace SystemGym.Application.Features.Clients.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para crear un cliente
/// </summary>
public class CreateClientCommand : ICommand<CreateClientCommandResult>
{
    public required string TipoDocumento { get; set; }
    public required string NumeroDocumento { get; set; }
    public required string NombreCompleto { get; set; }
    public string? Celular { get; set; }
}

/// <summary>
/// Resultado del comando de creación de cliente
/// </summary>
public class CreateClientCommandResult : CommandResult<Guid>
{
}
