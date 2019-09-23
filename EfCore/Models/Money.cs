using System;
using System.Collections.Generic;
using EfCore.Core;

namespace EfCore.Models
{
    // In case we need more values
    // https://github.com/zpbappi/money/blob/master/Money/Currency.cs
    // https://en.wikipedia.org/wiki/ISO_4217
    public enum Currency
    {
        Invalid = -1,
        EUR = 978,
        USD = 840
    }

    public class Money : ValueObject, IComparable<Money>
    {
        private const int DECIMALS = 2;

        public decimal Amount { get; private set; }
        public Currency Currency { get; private set; }

        public Money(decimal amount, Currency currency)
        {
            if (currency == Currency.Invalid)
                throw new ArgumentException("Invalid currency!");

            Amount = amount;
            Currency = currency;
        }

        public Money(decimal amount, string currencyName)
        {
            if (!Enum.TryParse<Currency>(currencyName, true, out var currency))
                throw new ArgumentException($"Unknown currency: {currencyName}!");

            Amount = amount;
            Currency = currency;
        }

        #region ValueObject implementation
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }
        #endregion

        // Shortcuts
        public static Money InEuro(decimal amount) => new Money(amount, Currency.EUR);
        public static explicit operator Money(decimal amount) => new Money(amount, Currency.EUR);
        public static explicit operator Money(double amount) => new Money((decimal)amount, Currency.EUR);

        // Operators
        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != left.Currency)
                throw new ArgumentException($"Cannot add different currencies: {left.Currency}, {right.Currency}");

            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != left.Currency)
                throw new ArgumentException($"Cannot add different currencies: {left.Currency}, {right.Currency}");

            return new Money(left.Amount - right.Amount, left.Currency);
        }

        // Constants
        public static readonly Money ZERO = InEuro(0);
        public static readonly Money CENT = InEuro(0.01m);

        // Methods
        public override string ToString() => $"{Amount:0.00} {Currency}";

        public Money GetPercentage(double percentage) => new Money(Math.Round(this.Amount * (decimal)percentage, DECIMALS), this.Currency);
        public Money Negative() => new Money(this.Amount * -1, this.Currency);

        // Comparable
        public static int Compare(Money left, Money right)
        {
            if (ReferenceEquals(left, null) & !ReferenceEquals(right, null)) return -1;
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
                return 0;
            if (ReferenceEquals(right, null)) return 1;

            return left.CompareTo(right);
        }

        public int CompareTo(Money other)
        {
            if (other.Currency != this.Currency)
                throw new ArgumentException("Cannot compare Money values of different currencies!");

            return this.Amount.CompareTo(other.Amount);
        }

        public static bool operator >(Money left, Money right) => Compare(left, right) == 1;
        public static bool operator <(Money left, Money right) => Compare(left, right) == -1;
        public static bool operator >=(Money left, Money right) => Compare(left, right) >= 0;
        public static bool operator <=(Money left, Money right) => Compare(left, right) <= 0;
    }
}