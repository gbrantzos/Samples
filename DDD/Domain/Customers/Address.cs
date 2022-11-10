using DDD.Domain.Core;

namespace DDD.Domain.Customers;

public class AddressID : EntityID
{
    public AddressID() { }

    public AddressID(int value) => Value = value;
}

public class Address : Entity<AddressID>
{
    public int Kind { get; set; }
    public string? Description { get; set; }
}