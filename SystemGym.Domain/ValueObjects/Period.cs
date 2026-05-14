namespace SystemGym.Domain.ValueObjects;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.Exceptions;

/// <summary>
/// Value Object para período (Día/Mes)
/// </summary>
public class Period : ValueObject
{
    public const string Dia = "Dia";
    public const string Mes = "Mes";

    private static readonly string[] ValidPeriods = { Dia, Mes };

    public string Value { get; private set; } = string.Empty;

    private Period() { } // EF Core

    private Period(string value)
    {
        Value = value;
    }

    public static Period Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Period cannot be empty");

        if (!ValidPeriods.Contains(value))
            throw new DomainException($"Period '{value}' is not valid. Valid values are: {string.Join(", ", ValidPeriods)}");

        return new Period(value);
    }

    public int GetDaysMultiplier() => Value switch
    {
        Dia => 1,
        Mes => 30,
        _ => throw new DomainException("Invalid period")
    };

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
