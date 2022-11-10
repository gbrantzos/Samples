using DDD.Domain.Core;

namespace DDD.Domain.Customers;

public interface ICustomerRepository : IRepository<Customer, CustomerID>
{
}