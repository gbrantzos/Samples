using System;
using System.Collections.Generic;

namespace EventSourcingSample
{
    public class Contact : EventSourcedAggregate
    {
        public Guid Id { get; protected set; }
        public string Fullname { get; protected set; }

        protected Contact() { }

        public Contact(string fullname)
        {
            Id = Guid.NewGuid();
            Fullname = fullname;
        }

        public Contact(IEnumerable<IEvent> eventStream)
        {
            foreach (var @event in eventStream)
                Apply(@event);
        }

        public override void Apply(IEvent @event)
        {
            When((dynamic)@event);
        }

        protected void When(ContactCreated @event)
        {
            Id = @event.Id;
            Fullname = @event.Fullname;
        }
    }
}
