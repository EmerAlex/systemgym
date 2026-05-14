namespace SystemGym.Domain.ValueObjects;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.Exceptions;
using System.Text.RegularExpressions;

/// <summary>
/// Value Object para número de teléfono
/// </summary>
public class PhoneNumber : ValueObject
{
    public string Value { get; private set; } = string.Empty;

    private PhoneNumber() { } // EF Core

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new PhoneNumber(string.Empty);

        // Validar formato: +57... o 10-15 dígitos
        if (!Regex.IsMatch(value, @"^\+?57\d{7,10}$|^\d{7,15}$"))
            throw new DomainException("Phone number format is invalid. Expected +57xxxxxxxxx or numeric");

        return new PhoneNumber(value.Trim());
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
