namespace SystemGym.Application.Features.Subscriptions.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Handler para el comando de registrar ingreso en suscripción
/// </summary>
public class RegisterIngresoCommandHandler : ICommandHandler<RegisterIngresoCommand, RegisterIngresoCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterIngresoCommandHandler> _logger;

    public RegisterIngresoCommandHandler(IUnitOfWork unitOfWork, ILogger<RegisterIngresoCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RegisterIngresoCommandResult> Handle(
        RegisterIngresoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _unitOfWork.Subscriptions
                .GetByIdAsync(request.SubscriptionId, cancellationToken);

            if (subscription is null)
                return new RegisterIngresoCommandResult
                {
                    Success = false,
                    Message = "Suscripción no encontrada",
                    Errors = new() { { "SubscriptionId", new[] { "Suscripción no encontrada" } } }
                };

            // RegisterIngreso valida: activa, tiene ingresos y no se registró hoy
            subscription.RegisterIngreso();

            _unitOfWork.Subscriptions.Update(subscription);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Ingreso registrado en suscripción {SubscriptionId}. Ingresos restantes: {Ingresos}",
                subscription.Id,
                subscription.CantidadIngresos);

            return new RegisterIngresoCommandResult
            {
                Success = true,
                Message = $"Ingreso registrado. Ingresos restantes: {subscription.CantidadIngresos}",
                Data = subscription.Id
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registro de ingreso rechazado para {SubscriptionId}: {Reason}",
                request.SubscriptionId, ex.Message);
            return new RegisterIngresoCommandResult
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar ingreso en suscripción {SubscriptionId}",
                request.SubscriptionId);
            return new RegisterIngresoCommandResult
            {
                Success = false,
                Message = $"Error al registrar el ingreso: {ex.Message}"
            };
        }
    }
}
