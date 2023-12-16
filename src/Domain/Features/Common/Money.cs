using System.Globalization;
using SimpleAPI.Core.Base;

namespace SimpleAPI.Domain.Features.Common;

// In case we need more values
// https://github.com/zpbappi/money/blob/master/Money/Currency.cs
// https://en.wikipedia.org/wiki/ISO_4217

// ReSharper disable InconsistentNaming
public enum Currency
{
    Invalid = 0,
    EUR = 978,
    USD = 840
}

public class Money : ValueObject
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Money(decimal amount, Currency currency)
    {
        if (currency == Currency.Invalid)
            throw new ArgumentException("Invalid currency!");

        Amount   = amount;
        Currency = currency;
    }

    public override string ToString() => $"{Amount.ToString("0.00", CultureInfo.InvariantCulture)} {Currency:G}";

    #region Factory, implicit operators

    public static Money Create(decimal amount, Currency currency) => new Money(amount, currency);
    public static Money InEuro(decimal amount) => new Money(amount, Currency.EUR);

    public static implicit operator Money(decimal amount) => new Money(amount, Currency.EUR);
    public static implicit operator Money(double amount) => new Money((decimal)amount, Currency.EUR);

    public static Money FromDouble(double value) => value;
    public static Money FromDecimal(decimal value) => value;

    #endregion

    #region Compare
    public override int CompareTo(ValueObject? other)
    {
        if (other == null) return 1;
        if (other is Money money && money.Currency != Currency)
            throw new ArgumentException("Cannot compare Money values of different currencies!");

        return base.CompareTo(other);
    }
    #endregion

    #region Equality
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
    #endregion

    #region Add/Subtract

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new ArgumentException($"Cannot add different currencies: {left.Currency}, {right.Currency}");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money Add(Money left, Money right) => left + right;

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new ArgumentException($"Cannot subtract different currencies: {left.Currency}, {right.Currency}");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money Subtract(Money left, Money right) => left - right;

    #endregion
}
