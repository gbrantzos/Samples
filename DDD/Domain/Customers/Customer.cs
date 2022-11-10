using DDD.Domain.Core;

namespace DDD.Domain.Customers;

public class CustomerID : EntityID
{
    public CustomerID() { }
    public CustomerID(int value) => Value = value;
}

public class Customer : Entity<CustomerID>
{
    public string Code { get; set; } = String.Empty;
    public string FullName { get; set; } = String.Empty;
    public string TIN { get; set; } = String.Empty;

    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.ToList();

    public void AddAddress(string street)
    {
        var address = new Address
        {
            Kind = 0,
            Description = street
        };
        _addresses.Add(address);
    }
}