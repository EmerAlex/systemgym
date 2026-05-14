namespace SystemGym.Domain.Entities;

using SystemGym.Domain.Abstractions;

/// <summary>
/// Entidad: Log de auditoría para ajustes de inventario (APPEND ONLY)
/// </summary>
public class InventoryLog : Entity
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }
    public int CantidadAnterior { get; private set; }
    public int CantidadNueva { get; private set; }
    public int Diferencia { get; private set; }
    public string Operacion { get; private set; } = string.Empty; // Add, Reduce, Adjust
    public string? Razon { get; private set; }

    private InventoryLog() { }

    private InventoryLog(Guid id, Guid productId, Guid userId, int cantAnterior, int cantNueva, 
        string operacion, string? razon) : base(id)
    {
        ProductId = productId;
        UserId = userId;
        CantidadAnterior = cantAnterior;
        CantidadNueva = cantNueva;
        Diferencia = cantNueva - cantAnterior;
        Operacion = operacion;
        Razon = razon;
    }

    public static InventoryLog Create(Guid productId, Guid userId, int cantidadAnterior, 
        int cantidadNueva, string operacion, string? razon = null)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty");

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty");

        if (cantidadNueva < 0)
            throw new ArgumentException("CantidadNueva cannot be negative");

        if (cantidadNueva == cantidadAnterior)
            throw new ArgumentException("CantidadNueva must be different from CantidadAnterior");

        return new InventoryLog(
            Guid.NewGuid(),
            productId,
            userId,
            cantidadAnterior,
            cantidadNueva,
            operacion,
            razon);
    }
}
