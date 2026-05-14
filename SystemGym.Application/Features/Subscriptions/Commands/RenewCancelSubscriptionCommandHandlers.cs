namespace SystemGym.Application.Features.Subscriptions.Commands;

using MediatR;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Handler para renovar una suscripción.
/// </summary>
public class RenewSubscriptionCommandHandler : ICommandHandler<RenewSubscriptionCommand, RenewSubscriptionCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public RenewSubscriptionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<RenewSubscriptionCommandResult> Handle(
        RenewSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(request.SubscriptionId, cancellationToken);

            if (subscription is null)
            {
                return new RenewSubscriptionCommandResult
                {
                    Success = false,
                    Message = "La suscripción no existe",
                    Errors = new() { { "SubscriptionId", ["Suscripción no encontrada"] } }
                };
            }

            var plan = await _unitOfWork.Plans.GetByIdAsync(subscription.PlanId, cancellationToken);

            if (plan is null)
            {
                return new RenewSubscriptionCommandResult
                {
                    Success = false,
                    Message = "El plan asociado no existe",
                    Errors = new() { { "PlanId", ["Plan no encontrado"] } }
                };
            }

            var nuevoFin = plan.CalculateFinDate(request.NuevoInicio, request.TieneExpiracion);
            var cantidadIngresos = Subscription.CalculateIngresos(plan.TipoPeriodo.Value, plan.CantidadIntervalosPeriodo);
            subscription.Renew(request.NuevoInicio, nuevoFin, request.TieneExpiracion, plan.Valor, cantidadIngresos);

            _unitOfWork.Subscriptions.Update(subscription);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in subscription.GetDomainEvents())
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            subscription.ClearDomainEvents();

            return new RenewSubscriptionCommandResult
            {
                Success = true,
                Message = "Suscripción renovada exitosamente",
                Data = subscription.Id
            };
        }
        catch (Exception ex)
        {
            return new RenewSubscriptionCommandResult
            {
                Success = false,
                Message = $"Error al renovar la suscripción: {ex.Message}"
            };
        }
    }
}

/// <summary>
/// Handler para cancelar una suscripción.
/// </summary>
public class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand, CancelSubscriptionCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelSubscriptionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelSubscriptionCommandResult> Handle(
        CancelSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(request.SubscriptionId, cancellationToken);

            if (subscription is null)
            {
                return new CancelSubscriptionCommandResult
                {
                    Success = false,
                    Message = "La suscripción no existe",
                    Errors = new() { { "SubscriptionId", ["Suscripción no encontrada"] } }
                };
            }

            if (!subscription.Activa)
            {
                return new CancelSubscriptionCommandResult
                {
                    Success = true,
                    Message = "La suscripción ya estaba cancelada",
                    Data = subscription.Id
                };
            }

            subscription.Cancel();
            _unitOfWork.Subscriptions.Update(subscription);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CancelSubscriptionCommandResult
            {
                Success = true,
                Message = "Suscripción cancelada exitosamente",
                Data = subscription.Id
            };
        }
        catch (Exception ex)
        {
            return new CancelSubscriptionCommandResult
            {
                Success = false,
                Message = $"Error al cancelar la suscripción: {ex.Message}"
            };
        }
    }
}
