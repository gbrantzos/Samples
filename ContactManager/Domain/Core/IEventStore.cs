using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactManager.Domain.Core
{
    public interface IEventStore
    {
        Task<IEnumerable<Event>> GetEventStream(long aggreagateID);

        Task SaveEventStream(IEnumerable<Event> events);
    }
}
