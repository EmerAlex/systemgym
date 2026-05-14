namespace SystemGym.Domain.Entities;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Entidad: Plan de suscripción
/// </summary>
public class Plan : Entity
{
    public string Descripcion { get; private set; } = string.Empty;
    public Period TipoPeriodo { get; private set; }
    public int CantidadIntervalosPeriodo { get; private set; }
    public decimal Valor { get; private set; }
    public bool Habilitado { get; private set; } = true;

    private Plan() { }

    private Plan(Guid id, string descripcion, Period tipoPeriodo, int cantidadIntervalos, decimal valor) 
        : base(id)
    {
        Descripcion = descripcion;
        TipoPeriodo = tipoPeriodo;
        CantidadIntervalosPeriodo = cantidadIntervalos;
        Valor = valor;
        Habilitado = true;
    }

    public static Plan Create(string descripcion, string tipoPeriodo, int cantidadIntervalosPeriodo, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Length > 200)
            throw new ArgumentException("Descripcion invalid");

        if (cantidadIntervalosPeriodo < 1 || cantidadIntervalosPeriodo > 999)
            throw new ArgumentException("CantidadIntervalosPeriodo must be between 1-999");

        if (valor < 0)
            throw new ArgumentException("Valor cannot be negative");

        return new Plan(
            Guid.NewGuid(),
            descripcion,
            Period.Create(tipoPeriodo),
            cantidadIntervalosPeriodo,
            valor);
    }

    public void Disable()
    {
        Habilitado = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string descripcion, string tipoPeriodo, int cantidadIntervalosPeriodo, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Length > 200)
            throw new ArgumentException("Descripcion invalid");

        if (cantidadIntervalosPeriodo < 1 || cantidadIntervalosPeriodo > 999)
            throw new ArgumentException("CantidadIntervalosPeriodo must be between 1-999");

        if (valor < 0)
            throw new ArgumentException("Valor cannot be negative");

        Descripcion = descripcion;
        TipoPeriodo = Period.Create(tipoPeriodo);
        CantidadIntervalosPeriodo = cantidadIntervalosPeriodo;
        Valor = valor;
        UpdatedAt = DateTime.UtcNow;
    }

    public DateTime? CalculateFinDate(DateTime inicioVigencia, bool tieneExpiracion)
    {
        if (!tieneExpiracion)
            return null;  // Null cuando no tiene expiración

        // Calcular según el tipo de período: meses exactos o días exactos
        return TipoPeriodo.Value switch
        {
            Period.Dia => inicioVigencia.AddDays(CantidadIntervalosPeriodo),
            Period.Mes => inicioVigencia.AddMonths(CantidadIntervalosPeriodo),
            _ => throw new ArgumentException($"Invalid period type: {TipoPeriodo}")
        };
    }
}
