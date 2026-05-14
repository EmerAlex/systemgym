namespace SystemGym.Application.Features.SystemUsers.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para crear un usuario del sistema
/// </summary>
public class CreateSystemUserCommand : ICommand<CreateSystemUserCommandResult>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}

/// <summary>
/// Resultado del comando de creación de usuario
/// </summary>
public class CreateSystemUserCommandResult : CommandResult<Guid>
{
}
