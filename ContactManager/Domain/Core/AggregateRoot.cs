using System;
using System.Collections.Generic;

namespace ContactManager.Domain.Core
{
    public abstract class AggregateRoot
    {
        protected List<Event> changes;

        public long ID     { get; set; }
        public int Version { get; }

        public int ApplicableVersion
            => this.Version + changes.Count + 1;

        protected AggregateRoot(long id)
        {
            ID = id;
            changes = new List<Event>();
        }

        protected AggregateRoot(long id, IEnumerable<Event> events) : this(id)
        {
            ID = id;

            // Apply history
            foreach (var @event in events)
                When(@event);
        }

        public void Apply(Event @event)
        {
            VersionGuard(@event);

            changes.Add(@event);
            When(@event);
        }

        public IEnumerable<Event> GetUnsavedChanges() => changes.AsReadOnly();
        public void ClearCommitted() => changes.Clear();

        protected abstract void When(Event @event);

        private void VersionGuard(Event @event)
        {
            if (@event.Version != this.ApplicableVersion)
                throw new ArgumentException($"Cannot apply event with version {@event.Version}.");
        }
    }
}
