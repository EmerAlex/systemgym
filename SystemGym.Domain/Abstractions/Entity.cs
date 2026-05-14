namespace SystemGym.Domain.Abstractions;

using MediatR;

/// <summary>
/// Interfaz base para todas las entidades del dominio
/// </summary>
public interface IEntity
{
    Guid Id { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
}

/// <summary>
/// Clase base para todas las entidades
/// </summary>
public abstract class Entity : IEntity
{
    private readonly List<INotification> _domainEvents = [];

    protected Entity() { }
    
    protected Entity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected void RaiseDomainEvent(INotification @event)
    {
        _domainEvents.Add(@event);
    }

    public IReadOnlyList<INotification> GetDomainEvents() => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    public override bool Equals(object? obj)
    {
        if (obj is not Entity entity)
            return false;

        return Id == entity.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>
/// Interfaz para raíces de agregados (entidades que pueden disparar eventos de dominio)
/// </summary>
public interface IAggregateRoot : IEntity
{
    IReadOnlyCollection<INotification> DomainEvents { get; }
    void ClearDomainEvents();
}
