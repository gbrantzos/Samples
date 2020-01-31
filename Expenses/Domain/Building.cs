using System;
using System.Collections.Generic;

namespace Expenses.Domain
{
    public class Building : Entity
    {
        public string Description { get; private set; }
        public Address Address { get; set; }

        private List<Apartment> apartments = new List<Apartment>();
        public IEnumerable<Apartment> Apartments => apartments.AsReadOnly();

        protected Building() { }

        public Building(string description, Address address)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public void AddApartment(Apartment toAdd)
        {
            this.apartments.Add(toAdd);
        }

        public void RemoveApartment(Apartment toRemove)
        {
            this.apartments.Remove(toRemove);
        }
    }
}
