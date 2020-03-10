using System;

namespace ContactManager.Domain.Core
{
    public abstract class Event
    {
        public Guid EventID     { get; }
        public long AggregateID { get; }
        public int Version      { get; }

        protected Event(long aggregateID, int version)
        {
            EventID     = Guid.NewGuid();
            AggregateID = aggregateID;
            Version     = version;
        }
    }
}