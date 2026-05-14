namespace SystemGym.Domain.Events;

using MediatR;

/// <summary>
/// Interfaz base para eventos de dominio
/// </summary>
public interface IDomainEvent : INotification
{
    Guid AggregateId { get; }
    DateTime OccurredAt { get; }
}

/// <summary>
/// Clase base para eventos de dominio
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid AggregateId { get; }
    public DateTime OccurredAt { get; }

    protected DomainEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
        OccurredAt = DateTime.UtcNow;
    }
}
