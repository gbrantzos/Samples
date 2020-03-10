using ContactManager.Domain.Core;

namespace ContactManager.Domain.Model
{
    public class ContactCreated : Event
    {
        public string FullName { get; set; }

        public ContactCreated(long aggregateID, int version, string fullName) : base(aggregateID, version)
        {
            this.FullName = fullName;
        }
    }
}
