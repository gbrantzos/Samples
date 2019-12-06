using System;

namespace EventSourcingSample
{
    public abstract class Event : IEvent
    {
        public Guid EventId { get; protected set; }
        public string EventType { get; protected set; }
        public Guid AggregateId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected Event(string eventType, Guid aggregateId)
        {
            AggregateId = aggregateId;
            EventType   = eventType;
            CreatedAt   = DateTime.UtcNow;
        }
    }
}