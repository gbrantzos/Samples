using ContactManager.Domain.Core;
using ContactManager.Utilities;

namespace ContactManager.Domain.Model
{
    public class ContactRepository : IAggregateRootRepository, IContactRepository
    {
        public long NextID()
        {
            return KeyGenerator.CreateKey<long>();
        }
    }
}
