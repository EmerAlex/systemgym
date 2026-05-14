namespace SystemGym.Domain.Abstractions;

/// <summary>
/// Clase base para raíces de agregados
/// Los agregados pueden disparar eventos de dominio
/// </summary>
public abstract class AggregateRoot : Entity
{
    protected AggregateRoot() { }

    protected AggregateRoot(Guid id) : base(id) { }
}
