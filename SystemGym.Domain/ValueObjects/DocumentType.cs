namespace SystemGym.Domain.ValueObjects;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.Exceptions;

/// <summary>
/// Value Object para tipo de documento
/// </summary>
public class DocumentType : ValueObject
{
    public const string CC = "CC";
    public const string TI = "TI";
    public const string CE = "CE";
    public const string PAS = "PAS";

    private static readonly string[] ValidTypes = { CC, TI, CE, PAS };

    public string Value { get; private set; } = string.Empty;

    private DocumentType() { } // EF Core

    private DocumentType(string value)
    {
        Value = value;
    }

    public static DocumentType Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Document type cannot be empty");

        if (!ValidTypes.Contains(value))
            throw new DomainException($"Document type '{value}' is not valid. Valid types are: {string.Join(", ", ValidTypes)}");

        return new DocumentType(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
