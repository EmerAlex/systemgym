namespace SystemGym.Domain.Entities;

using SystemGym.Domain.Abstractions;

/// <summary>
/// Entidad: Producto del gimnasio
/// </summary>
public class Product : Entity
{
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public bool Habilitado { get; private set; } = true;
    public int CantidadActual { get; private set; } = 0;

    private Product() { }

    private Product(Guid id, string descripcion, decimal valor) : base(id)
    {
        Descripcion = descripcion;
        Valor = valor;
        Habilitado = true;
        CantidadActual = 0;
    }

    public static Product Create(string descripcion, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Length > 200)
            throw new ArgumentException("Descripcion invalid");

        if (valor < 0)
            throw new ArgumentException("Valor cannot be negative");

        return new Product(Guid.NewGuid(), descripcion, valor);
    }

    public void Disable()
    {
        Habilitado = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string descripcion, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Length > 200)
            throw new ArgumentException("Descripcion invalid");

        if (valor < 0)
            throw new ArgumentException("Valor cannot be negative");

        Descripcion = descripcion;
        Valor = valor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInventory(int cantidadNueva)
    {
        if (cantidadNueva < 0)
            throw new ArgumentException("Cantidad cannot be negative");

        CantidadActual = cantidadNueva;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Descuenta una unidad del stock. Lanza excepción si no hay stock disponible.
    /// </summary>
    public void DecrementStock()
    {
        if (CantidadActual <= 0)
            throw new InvalidOperationException("Sin stock disponible para este producto");

        CantidadActual--;
        UpdatedAt = DateTime.UtcNow;
    }
}
