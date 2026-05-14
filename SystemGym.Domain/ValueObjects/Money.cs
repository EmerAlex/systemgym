namespace SystemGym.Domain.ValueObjects;

using SystemGym.Domain.Abstractions;
using SystemGym.Domain.Exceptions;

/// <summary>
/// Value Object para dinero/valor monetario
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency = "COP")
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "COP")
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");

        if (amount > 999999999.99m)
            throw new DomainException("Amount exceeds maximum value");

        return new Money(amount, currency);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot add amounts in different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new DomainException("Cannot subtract amounts in different currencies");

        return Create(left.Amount - right.Amount, left.Currency);
    }
}
