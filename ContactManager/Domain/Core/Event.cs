using System;
using MediatR;

namespace ContactManager.Domain.Core
{
    public abstract class Event : INotification
    {
        public Guid EventID       { get; }
        public long AggregateID   { get; }
        public int Version        { get; }
        public DateTime CreatedAt { get; }

        protected Event(long aggregateID, int version)
        {
            EventID     = Guid.NewGuid();
            CreatedAt   = DateTime.UtcNow;
            AggregateID = aggregateID;
            Version     = version;
        }
    }
}