using System;
using System.Threading.Tasks;

namespace EventSourcingSample
{
    public class ContactRepository : IContactRepository
    {
        private readonly IEventStore eventStore;

        public ContactRepository(IEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        public async Task<Contact> GetById(Guid id)
        {
            var events = await eventStore.GetEventStream(id);
            var contact = new Contact(events);

            return contact;
        }

        public Task Save(Contact contact)
        {
            var events = contact.UncommitedEvents();
            return eventStore.SaveEventStream(events);
        }
    }
}
