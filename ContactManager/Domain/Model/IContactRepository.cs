using System.Threading.Tasks;
using ContactManager.Domain.Core;

namespace ContactManager.Domain.Model
{
    public interface IContactRepository : IAggregateRootRepository
    {
        Task<Contact> GetByID(long id);

        Task Save(Contact contact);
    }
}