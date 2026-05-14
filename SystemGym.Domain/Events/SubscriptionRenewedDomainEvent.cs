namespace SystemGym.Domain.Events;

/// <summary>
/// Evento de dominio: Suscripción renovada.
/// Se dispara cuando una suscripción se renueva y debe generar una venta automática.
/// </summary>
public class SubscriptionRenewedDomainEvent : DomainEvent
{
    public Guid ClientId { get; }
    public Guid PlanId { get; }
    public decimal PlanValue { get; }
    public DateTime StartDate { get; }

    public SubscriptionRenewedDomainEvent(
        Guid subscriptionId,
        Guid clientId,
        Guid planId,
        decimal planValue,
        DateTime startDate)
        : base(subscriptionId)
    {
        ClientId = clientId;
        PlanId = planId;
        PlanValue = planValue;
        StartDate = startDate;
    }
}
