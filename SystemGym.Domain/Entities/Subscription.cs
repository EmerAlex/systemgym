namespace SystemGym.Domain.Entities;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.Events;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Agregado raíz: Suscripción del cliente a un plan
/// </summary>
public class Subscription : AggregateRoot
{
    public Guid ClientId { get; private set; }
    public Guid PlanId { get; private set; }
    public DateTime InicioVigencia { get; private set; }
    public DateTime? FinVigencia { get; private set; }  // Nullable si TieneExpiracion es false
    public bool TieneExpiracion { get; private set; } = true;
    public bool Activa { get; private set; } = true;
    public DateTime? UltimoIngreso { get; private set; }
    public int CantidadIngresos { get; private set; } = 0;
    public decimal Valor { get; private set; } // Denormalizado para auditoría

    private Subscription() { }

    private Subscription(Guid id, Guid clientId, Guid planId, DateTime inicioVigencia, DateTime? finVigencia,
        bool tieneExpiracion, decimal valor, int cantidadIngresos) : base(id)
    {
        ClientId = clientId;
        PlanId = planId;
        InicioVigencia = ToUtcDate(inicioVigencia);
        FinVigencia = finVigencia.HasValue ? ToUtcDate(finVigencia.Value) : null;
        TieneExpiracion = tieneExpiracion;
        Valor = valor;
        CantidadIngresos = cantidadIngresos;
        Activa = CalculateActiveStatus();
    }

    /// <summary>
    /// Calcula la cantidad de ingresos según la periodicidad del plan.
    /// Mes: intervalos * 30 días. Día: intervalos exactos.
    /// </summary>
    public static int CalculateIngresos(string tipoPeriodo, int cantidadIntervalos)
    {
        return tipoPeriodo == Period.Mes
            ? cantidadIntervalos * 30
            : cantidadIntervalos;
    }

    public static Subscription Create(Guid clientId, Guid planId, DateTime inicioVigencia,
        bool tieneExpiracion, DateTime? finVigencia, decimal planValor, int cantidadIngresos)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("ClientId cannot be empty");

        if (planId == Guid.Empty)
            throw new ArgumentException("PlanId cannot be empty");

        if (inicioVigencia < DateTime.Today)
            throw new ArgumentException("InicioVigencia cannot be before today");

        if (tieneExpiracion && (!finVigencia.HasValue || finVigencia.Value <= inicioVigencia))
            throw new ArgumentException("FinVigencia must be after InicioVigencia when TieneExpiracion is true");

        var subscription = new Subscription(
            Guid.NewGuid(),
            clientId,
            planId,
            inicioVigencia,
            finVigencia,
            tieneExpiracion,
            planValor,
            cantidadIngresos);

        // Disparar evento de dominio para que se cree la venta automáticamente
        var @event = new SubscriptionCreatedDomainEvent(
            subscription.Id,
            clientId,
            planId,
            planValor,
            inicioVigencia);

        subscription.RaiseDomainEvent(@event);

        return subscription;
    }

    public void Renew(DateTime nuevoInicio, DateTime? nuevoFin, bool tieneExpiracion, decimal planValor, int cantidadIngresos)
    {
        if (nuevoInicio < DateTime.Today)
            throw new ArgumentException("NuevoInicio cannot be before today");

        if (tieneExpiracion && (!nuevoFin.HasValue || nuevoFin.Value <= nuevoInicio))
            throw new ArgumentException("NuevoFin must be after NuevoInicio when TieneExpiracion is true");

        if (cantidadIngresos < 0)
            throw new ArgumentException("CantidadIngresos cannot be negative");

        InicioVigencia = ToUtcDate(nuevoInicio);
        FinVigencia = nuevoFin.HasValue ? ToUtcDate(nuevoFin.Value) : null;
        TieneExpiracion = tieneExpiracion;
        Valor = planValor;
        CantidadIngresos = cantidadIngresos; // Resetea al valor del plan (igual que en Create)
        UltimoIngreso = null;               // Nueva vigencia, sin ingresos registrados aún
        Activa = CalculateActiveStatus();
        UpdatedAt = DateTime.UtcNow;

        var @event = new SubscriptionRenewedDomainEvent(
            Id,
            ClientId,
            PlanId,
            planValor,
            nuevoInicio);

        RaiseDomainEvent(@event);
    }

    public void Cancel()
    {
        Activa = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Evalúa si la suscripción debe pasar a inactiva por:
    /// 1. Fecha de expiración vencida (FinVigencia &lt;= hoy)
    /// 2. Sin ingresos disponibles (CantidadIngresos == 0)
    /// Retorna true si el estado cambió (para disparar un Update en BD).
    /// </summary>
    public bool EvaluateAndDeactivateIfNeeded()
    {
        if (!Activa) return false; // Ya está inactiva, nada que hacer

        var today = DateTime.UtcNow.Date;
        var shouldDeactivate = false;

        if (TieneExpiracion && FinVigencia.HasValue && FinVigencia.Value.Date <= today)
            shouldDeactivate = true;

        if (CantidadIngresos == 0)
            shouldDeactivate = true;

        if (shouldDeactivate)
        {
            Activa = false;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Registra un ingreso del cliente: valida 1 por día y que queden ingresos disponibles.
    /// </summary>
    public void RegisterIngreso()
    {
        if (!Activa)
            throw new InvalidOperationException("La suscripción no está activa");

        if (CantidadIngresos <= 0)
            throw new InvalidOperationException("La suscripción no tiene ingresos disponibles");

        if (UltimoIngreso.HasValue &&
            UltimoIngreso.Value.ToUniversalTime().Date == DateTime.UtcNow.Date)
            throw new InvalidOperationException("Ya se registró un ingreso hoy para esta suscripción");

        CantidadIngresos--;
        UltimoIngreso = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private bool CalculateActiveStatus()
    {
        var today = DateTime.UtcNow.Date;
        
        // Si no tiene expiración, solo verificar que sea >= inicio
        if (!TieneExpiracion)
            return today >= InicioVigencia.Date;
        
        // Si tiene expiración, verificar ambos límites
        return today >= InicioVigencia.Date && FinVigencia.HasValue && today <= FinVigencia.Value.Date;
    }

    /// <summary>
    /// Convierte cualquier DateTime a UTC con Kind=Utc para compatibilidad con PostgreSQL timestamp with time zone.
    /// Trata el valor como fecha pura (solo año/mes/día, sin componente de hora).
    /// </summary>
    private static DateTime ToUtcDate(DateTime dt)
        => dt == DateTime.MaxValue
            ? DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc)
            : DateTime.SpecifyKind(dt.Date, DateTimeKind.Utc);
}
