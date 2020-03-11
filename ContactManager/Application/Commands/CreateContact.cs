using System.Threading;
using System.Threading.Tasks;
using ContactManager.Domain.Model;
using MediatR;

namespace ContactManager.Application.Commands
{
    public static class CreateContact
    {
        public class Command : IRequest<bool>
        {
            public string FullName { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly IContactRepository contactRepository;

            public Handler(IContactRepository contactRepository)
                => this.contactRepository = contactRepository;

            public Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var contactID = contactRepository.NextID();

                var @event = new ContactCreated(contactID, 1, request.FullName);
                var contact = new Contact(contactID);

                contact.Apply(@event);
                contactRepository.Save(contact);

                return Task.FromResult(true);
            }
        }
    }
}
