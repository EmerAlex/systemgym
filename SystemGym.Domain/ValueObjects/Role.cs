namespace SystemGym.Domain.ValueObjects;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.Exceptions;

/// <summary>
/// Value Object para rol de usuario
/// </summary>
public class Role : ValueObject
{
    public const string Admin = "Admin";
    public const string Standard = "Standard";

    private static readonly string[] ValidRoles = { Admin, Standard };

    public string Value { get; private set; } = string.Empty;

    private Role() { } // EF Core

    private Role(string value)
    {
        Value = value;
    }

    public static Role Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Role cannot be empty");

        if (!ValidRoles.Contains(value))
            throw new DomainException($"Role '{value}' is not valid. Valid roles are: {string.Join(", ", ValidRoles)}");

        return new Role(value);
    }

    public bool IsAdmin => Value == Admin;
    public bool IsStandard => Value == Standard;

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
