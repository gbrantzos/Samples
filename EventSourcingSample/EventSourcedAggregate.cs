using System.Collections.Generic;

namespace EventSourcingSample
{
    public abstract class EventSourcedAggregate : IEventSourcedAggregate
    {
        protected List<IEvent> events = new List<IEvent>();

        public abstract void Apply(IEvent @event);

        public IEnumerable<IEvent> UncommitedEvents() => events;

        public void AddEvent(IEvent @event) => events.Add(@event);
    }
}
