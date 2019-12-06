using System.Collections.Generic;

namespace EventSourcingSample
{
    public interface IEventSourcedAggregate
    {
        void Apply(IEvent @event);

        IEnumerable<IEvent> UncommitedEvents();
        void AddEvent(IEvent @event);
    }
}