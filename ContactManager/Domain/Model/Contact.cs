using System.Collections.Generic;
using ContactManager.Domain.Core;

namespace ContactManager.Domain.Model
{
    public class Contact : AggregateRoot
    {
        public string FullName { get; private set; }

        public Contact(long id) : base(id) { }
        public Contact(long id, IEnumerable<Event> events) : base(id, events) { }

        protected override void When(Event @event) => When((dynamic)@event);

        #region Handle events
        private void When(ContactCreated contactCreated)
            => this.FullName = contactCreated.FullName;
        #endregion
    }
}
