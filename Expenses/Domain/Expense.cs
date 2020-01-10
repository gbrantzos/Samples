using System;
using System.Collections.Generic;

namespace Expenses.Domain
{
    public class Expense
    {
        public long ExpenseId     { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime DueAt     { get; private set; }
        public string Description { get; private set; }
        public Money Amount       { get; private set; }

        protected Expense() { }

        public Expense(DateTime createdAt, DateTime dueAt, string description, Money amount)
        {
            ExpenseId   = KeyGenerator.NewId();
            CreatedAt   = createdAt;
            DueAt       = dueAt;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Amount      = amount ?? throw new ArgumentNullException(nameof(amount));
        }
    }

    public class Money
    {
        private static readonly HashSet<string> KnownCurrencies = new HashSet<string>
        {
            "EUR",
            "USD"
        };

        public string Currency { get; private set; }
        public decimal Value   { get; private set; }

        protected Money() { }

        public Money(string currency, decimal value)
        {
            if (!KnownCurrencies.Contains(currency))
                throw new ArgumentException($"Unknown currency: {currency}");
            Currency = currency;
            Value = value;
        }

        public static Money Euro(decimal value) => new Money("EUR", value);
    }
}
