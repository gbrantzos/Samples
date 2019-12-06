using System;

namespace EventSourcingSample
{
    public class ContactCreated : Event
    {
        public string Fullname { get; }
        public Guid Id { get; }

        public ContactCreated(Guid id, string fullname) : base(ContactEvents.Created, id)
        {
            Fullname = fullname;
            Id = id;
        }
    }
}