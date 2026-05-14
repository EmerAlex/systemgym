namespace SystemGym.Application.Features.Subscriptions.EventHandlers;

using MediatR;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;
using SystemGym.Domain.Events;

/// <summary>
/// Handler para el evento SubscriptionCreatedDomainEvent.
/// Genera automáticamente una venta cuando se crea una suscripción.
/// </summary>
public class SubscriptionCreatedEventHandler : INotificationHandler<SubscriptionCreatedDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubscriptionCreatedEventHandler> _logger;

    public SubscriptionCreatedEventHandler(IUnitOfWork unitOfWork, ILogger<SubscriptionCreatedEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(SubscriptionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await CreateSubscriptionSaleAsync(
            notification.AggregateId,
            notification.ClientId,
            notification.PlanId,
            notification.PlanValue,
            "SubscriptionCreatedDomainEvent",
            cancellationToken);
    }

    protected async Task CreateSubscriptionSaleAsync(
        Guid subscriptionId,
        Guid clientId,
        Guid planId,
        decimal planValue,
        string source,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Procesando evento {Source}. SubscriptionId: {SubscriptionId}, ClientId: {ClientId}, PlanId: {PlanId}",
                source,
                subscriptionId,
                clientId,
                planId);

            var sale = SalesHistory.CreateFromSubscription(
                clientId,
                planId,
                subscriptionId,
                null, // Usuario del sistema (operación automática) - null es válido
                planValue);

            _unitOfWork.SalesHistory.Add(sale);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Venta generada automáticamente. SaleId: {SaleId}, Monto: {Monto}",
                sale.Id,
                sale.Monto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar evento {Source}", source);
            throw;
        }
    }
}

/// <summary>
/// Handler para el evento SubscriptionRenewedDomainEvent.
/// Genera automáticamente una venta cuando se renueva una suscripción.
/// </summary>
public class SubscriptionRenewedEventHandler : INotificationHandler<SubscriptionRenewedDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubscriptionCreatedEventHandler> _logger;

    public SubscriptionRenewedEventHandler(IUnitOfWork unitOfWork, ILogger<SubscriptionCreatedEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(SubscriptionRenewedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Procesando evento SubscriptionRenewedDomainEvent. SubscriptionId: {SubscriptionId}, ClientId: {ClientId}, PlanId: {PlanId}",
                notification.AggregateId,
                notification.ClientId,
                notification.PlanId);

            var sale = SalesHistory.CreateFromSubscription(
                notification.ClientId,
                notification.PlanId,
                notification.AggregateId,
                null, // Usuario del sistema (operación automática)
                notification.PlanValue);

            _unitOfWork.SalesHistory.Add(sale);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Venta generada automáticamente por renovación. SaleId: {SaleId}, Monto: {Monto}",
                sale.Id,
                sale.Monto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar evento SubscriptionRenewedDomainEvent");
            throw;
        }
    }
}
