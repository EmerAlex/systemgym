namespace SystemGym.Domain.Entities;

using SystemGym.Domain.Abstractions;

/// <summary>
/// Entidad: Historial de ventas
/// </summary>
public class SalesHistory : Entity
{
    public Guid ClientId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? SubscriptionId { get; private set; } // Null si es venta manual
    public Guid? UserId { get; private set; } // Usuario que realizó la venta (null si es generada por sistema)
    public DateTime FechaVenta { get; private set; }
    public decimal Monto { get; private set; }
    public bool Pagado { get; private set; }
    public string? MedioPago { get; private set; }
    public string? Referencia { get; private set; }

    private SalesHistory() { }

    private SalesHistory(Guid id, Guid clientId, Guid productId, Guid? userId, DateTime fechaVenta, 
        decimal monto, bool pagado, string? medioPago, string? referencia, Guid? subscriptionId = null) 
        : base(id)
    {
        ClientId = clientId;
        ProductId = productId;
        UserId = userId;
        SubscriptionId = subscriptionId;
        FechaVenta = fechaVenta;
        Monto = monto;
        Pagado = pagado;
        MedioPago = medioPago;
        Referencia = referencia;
    }

    public static SalesHistory Create(Guid clientId, Guid productId, Guid? userId, DateTime fechaVenta, 
        decimal monto, bool pagado, string? medioPago = null, string? referencia = null)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("ClientId cannot be empty");

        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty");

        // UserId puede ser null para operaciones del sistema

        if (fechaVenta > DateTime.UtcNow)
            throw new ArgumentException("FechaVenta cannot be in the future");

        if (monto < 0)
            throw new ArgumentException("Monto cannot be negative");

        return new SalesHistory(
            Guid.NewGuid(),
            clientId,
            productId,
            userId,
            fechaVenta,
            monto,
            pagado,
            medioPago,
            referencia);
    }

    public static SalesHistory CreateFromSubscription(Guid clientId, Guid productId, 
        Guid subscriptionId, Guid? userId, decimal monto)
    {
        if (subscriptionId == Guid.Empty)
            throw new ArgumentException("SubscriptionId cannot be empty");

        var sale = new SalesHistory(
            Guid.NewGuid(),
            clientId,
            productId,
            userId,
            DateTime.UtcNow,
            monto,
            true, // Siempre pagado automáticamente
            "Subscription",
            $"SUB-{subscriptionId}",
            subscriptionId);

        return sale;
    }

    public void MarkAsPaid(string? medioPago = null, string? referencia = null)
    {
        if (Pagado)
            throw new InvalidOperationException("Sale is already paid");

        Pagado = true;
        MedioPago = medioPago;
        Referencia = referencia;
        UpdatedAt = DateTime.UtcNow;
    }
}
