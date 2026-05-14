namespace SystemGym.Application.Features.SystemUsers.Commands;

using MediatR;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Handler para el comando de crear usuario del sistema
/// </summary>
public class CreateSystemUserCommandHandler : ICommandHandler<CreateSystemUserCommand, CreateSystemUserCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateSystemUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateSystemUserCommandResult> Handle(
        CreateSystemUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validar que no exista usuario con el mismo nombre de usuario
            var existingUser = await _unitOfWork.SystemUsers
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (existingUser is not null)
                return new CreateSystemUserCommandResult
                {
                    Success = false,
                    Message = "Ya existe un usuario con ese nombre de usuario",
                    Errors = new() { { "Username", new[] { "Usuario duplicado" } } }
                };

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Crear el usuario del sistema
            var newUser = SystemUser.Create(
                request.Username,
                passwordHash,
                string.Empty,
                request.Role);

            _unitOfWork.SystemUsers.Add(newUser);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateSystemUserCommandResult
            {
                Success = true,
                Message = "Usuario creado exitosamente",
                Data = newUser.Id
            };
        }
        catch (Exception ex)
        {
            return new CreateSystemUserCommandResult
            {
                Success = false,
                Message = $"Error al crear el usuario: {ex.Message}"
            };
        }
    }
}
