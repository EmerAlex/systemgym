namespace SystemGym.Application.Features.Subscriptions.Commands;

using MediatR;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Handler para el comando de crear suscripción
/// Importante: Al crear una suscripción, se genera automáticamente una venta asociada
/// </summary>
public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, CreateSubscriptionCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateSubscriptionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<CreateSubscriptionCommandResult> Handle(
        CreateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validar que el cliente existe
            var client = await _unitOfWork.Clients
                .FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);

            if (client is null)
                return new CreateSubscriptionCommandResult
                {
                    Success = false,
                    Message = "El cliente no existe",
                    Errors = new() { { "ClientId", new[] { "Cliente no encontrado" } } }
                };

            // Validar que el plan existe
            var plan = await _unitOfWork.Plans
                .GetByIdAsync(request.PlanId, cancellationToken);

            if (plan is null)
                return new CreateSubscriptionCommandResult
                {
                    Success = false,
                    Message = "El plan no existe",
                    Errors = new() { { "PlanId", new[] { "Plan no encontrado" } } }
                };

            // Validar que no exista suscripción activa del cliente a este plan
            var existingSubscription = await _unitOfWork.Subscriptions
                .FirstOrDefaultAsync(s => s.ClientId == request.ClientId &&
                                          s.PlanId == request.PlanId &&
                                          s.Activa, cancellationToken);

            if (existingSubscription is not null)
                return new CreateSubscriptionCommandResult
                {
                    Success = false,
                    Message = "El cliente ya tiene una suscripción activa a este plan"
                };

            // Calcular cantidad de ingresos según periodicidad del plan
            var cantidadIngresos = Subscription.CalculateIngresos(
                plan.TipoPeriodo.Value,
                plan.CantidadIntervalosPeriodo);

            // Calcular fecha de vencimiento solo si tiene expiración
            DateTime? finVigencia = null;
            if (request.TieneExpiracion)
            {
                finVigencia = plan.CalculateFinDate(request.InicioVigencia, true);
            }

            // Crear la suscripción
            var subscription = Subscription.Create(
                request.ClientId,
                request.PlanId,
                request.InicioVigencia,
                request.TieneExpiracion,
                finVigencia,
                plan.Valor,
                cantidadIngresos);

            _unitOfWork.Subscriptions.Add(subscription);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Procesar eventos de dominio (genera venta automáticamente)
            foreach (var domainEvent in subscription.GetDomainEvents())
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            return new CreateSubscriptionCommandResult
            {
                Success = true,
                Message = "Suscripción creada exitosamente",
                Data = subscription.Id
            };
        }
        catch (Exception ex)
        {
            return new CreateSubscriptionCommandResult
            {
                Success = false,
                Message = $"Error al crear la suscripción: {ex.Message}"
            };
        }
    }
}
