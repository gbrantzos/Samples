using System;
using MediatR;

namespace EventSourcingSample
{
    public interface IEvent : INotification
    {
        public Guid EventId { get; }

        public string EventType { get; }

        public Guid AggregateId { get; }
        public DateTime CreatedAt { get; }
    }
}
