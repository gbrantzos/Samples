using System;

namespace Expenses.Domain
{

    public class Address
    {
        public string Street { get; private set; }

        public Address(string street)
        {
            Street = street ?? throw new ArgumentNullException(nameof(street));
        }
    }
}
