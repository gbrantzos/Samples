using System.Threading.Tasks;
using ContactManager.Domain.Core;
using ContactManager.Domain.Model;
using ContactManager.Utilities;
using MediatR;

namespace ContactManager.Infrastructure.Persistence
{
    public class ContactRepository : IContactRepository
    {
        private readonly IEventStore eventStore;
        private readonly IMediator mediator;

        public ContactRepository(IMediator mediator, IEventStore eventStore)
        {
            this.mediator   = mediator;
            this.eventStore = eventStore;
        }

        public async Task<Contact> GetByID(long id)
        {
            var eventStream = await eventStore.GetEventStream(id);
            return new Contact(id, eventStream);
        }

        public long NextID()
            => KeyGenerator.CreateKey<long>();

        public async Task Save(Contact contact)
        {
            var eventStream = contact.GetUnsavedChanges();
            await eventStore.SaveEventStream(eventStream);
            contact.ClearChanges();

            foreach (var @event in eventStream)
                _ = mediator.Send(@event);
        }
    }
}
