using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcingSample
{
    public class InMemoryEventStore : IEventStore
    {
        public Task<IEnumerable<IEvent>> GetEventStream(Guid aggreagateId)
        {
            var events = new[]
            {
                new ContactCreated(Guid.NewGuid(), "Giorgio")
            };
            return Task.FromResult(events.AsEnumerable<IEvent>());
        }

        public Task SaveEventStream(IEnumerable<IEvent> events)
        {
            throw new NotImplementedException();
        }
    }
}
