namespace SystemGym.Domain.Events;

/// <summary>
/// Evento de dominio: Suscripción creada
/// Se dispara cuando un cliente crea una nueva suscripción a un plan
/// </summary>
public class SubscriptionCreatedDomainEvent : DomainEvent
{
    public Guid ClientId { get; }
    public Guid PlanId { get; }
    public decimal PlanValue { get; }
    public DateTime StartDate { get; }

    public SubscriptionCreatedDomainEvent(
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
