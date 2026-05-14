namespace SystemGym.Application.Features.Clients.Commands;

using MediatR;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Handler para el comando de crear cliente
/// </summary>
public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, CreateClientCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateClientCommandResult> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validar que no exista cliente con el mismo documento
            var existingClient = await _unitOfWork.Clients
                .FirstOrDefaultAsync(c => c.NumeroDocumento == request.NumeroDocumento, cancellationToken);

            if (existingClient is not null)
                return new CreateClientCommandResult
                {
                    Success = false,
                    Message = "Ya existe un cliente con ese número de documento",
                    Errors = new() { { "NumeroDocumento", new[] { "Documento duplicado" } } }
                };

            // Crear cliente
            var client = Client.Create(
                request.TipoDocumento,
                request.NumeroDocumento,
                request.NombreCompleto,
                request.Celular);

            _unitOfWork.Clients.Add(client);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateClientCommandResult
            {
                Success = true,
                Message = "Cliente creado exitosamente",
                Data = client.Id
            };
        }
        catch (Exception ex)
        {
            return new CreateClientCommandResult
            {
                Success = false,
                Message = $"Error al crear el cliente: {ex.Message}"
            };
        }
    }
}
