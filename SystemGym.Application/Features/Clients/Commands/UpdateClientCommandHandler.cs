namespace SystemGym.Application.Features.Clients.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Handler para el comando de actualizar cliente
/// </summary>
public class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand, UpdateClientCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateClientCommandHandler> _logger;

    public UpdateClientCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateClientCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdateClientCommandResult> Handle(
        UpdateClientCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId, cancellationToken);
            if (client is null)
                return new UpdateClientCommandResult { Success = false, Message = "Cliente no encontrado" };

            // Validar que no exista otro cliente con el mismo documento (si cambió el número)
            if (client.NumeroDocumento != request.NumeroDocumento)
            {
                var existingClient = await _unitOfWork.Clients
                    .FirstOrDefaultAsync(c => c.NumeroDocumento == request.NumeroDocumento, cancellationToken);

                if (existingClient is not null)
                    return new UpdateClientCommandResult
                    {
                        Success = false,
                        Message = "Ya existe un cliente con ese número de documento",
                        Errors = new() { { "NumeroDocumento", new[] { "Documento duplicado" } } }
                    };
            }

            // Actualizar todos los campos
            client.UpdateClient(
                request.TipoDocumento,
                request.NumeroDocumento,
                request.NombreCompleto,
                request.Celular,
                request.Habilitado);

            _unitOfWork.Clients.Update(client);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cliente actualizado: {ClientId}", client.Id);

            return new UpdateClientCommandResult
            {
                Success = true,
                Message = "Cliente actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cliente {ClientId}", request.ClientId);
            return new UpdateClientCommandResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}
