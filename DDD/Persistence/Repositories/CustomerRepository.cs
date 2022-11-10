using DDD.Domain.Customers;

namespace DDD.Persistence.Repositories;

public class CustomerRepository : GenericRepository<Customer, CustomerID>, ICustomerRepository
{
    public CustomerRepository(Context context) : base(context)
    {
    }

    protected override IEnumerable<string> NavigationProperties => new[] {nameof(Customer.Addresses)};
}