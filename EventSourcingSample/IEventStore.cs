using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcingSample
{
    public interface IEventStore
    {
        Task<IEnumerable<IEvent>> GetEventStream(Guid aggreagateId);

        Task SaveEventStream(IEnumerable<IEvent> events);
    }
}
