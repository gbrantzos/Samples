using System;
using EfCore.Core;

namespace EfCore.Models
{
    public enum ExpenseCategory
    {
        Heating  = 1,
        Elevator = 2,
        Shared   = 3
    }

    public class ExpenseID : EntityID { }

    public class Expense
    {
        public ExpenseID ExpenseID      { get; private set; }
        public string Description       { get; private set; }
        public ExpenseCategory Category { get; private set; }
        public DateTime IssuedAt        { get; private set; }
        public Money Amount             { get; private set; }
        public bool ForOwner            { get; private set; }

        protected Expense() { }

        public Expense(string description, bool forOwner, DateTime issuedAt, Money amount, ExpenseCategory category)
        {
            // ExpenseID = EntityID.CreateNew<ExpenseID>();
            IssuedAt    = issuedAt;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Amount      = amount ?? throw new ArgumentNullException(nameof(amount));
            Category    = category;
            ForOwner    = forOwner;
        }

        public override string ToString() => $"Expense: {Description} on {Category.ToString()}, {Amount}, {IssuedAt}";
    }
}
