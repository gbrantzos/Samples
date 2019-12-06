using System;
using System.Threading.Tasks;

namespace EventSourcingSample
{
    public interface IContactRepository
    {
        Task<Contact> GetById(Guid id);

        Task Save(Contact contact);
    }
}
