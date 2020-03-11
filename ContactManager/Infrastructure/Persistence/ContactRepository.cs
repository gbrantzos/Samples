using System.Collections.Generic;
using System.Linq;
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
            var tasks = new List<Task>();

            var eventStream = contact
                .GetUnsavedChanges()
                .ToList();
            contact.ClearChanges();

            var saveTask = eventStore.SaveEventStream(eventStream);
            tasks.Add(saveTask);

            foreach (var @event in eventStream)
            {
                var publishTask = mediator.Publish(@event);
                tasks.Add(publishTask);
            }

            // Wait all tasks to avoid weird exceptions!
            await Task.WhenAll(tasks);
        }
    }
}
