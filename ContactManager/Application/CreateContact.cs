using System.Threading;
using System.Threading.Tasks;
using ContactManager.Domain.Model;
using MediatR;

namespace ContactManager.Application
{
    public static class CreateContact
    {
        public class Command : IRequest<bool>
        {
            public string FullName { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly IMediator mediator;
            private readonly IContactRepository contactRepository;

            public Handler(IMediator mediator, IContactRepository contactRepository)
            {
                this.mediator = mediator;
                this.contactRepository = contactRepository;
            }

            public Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var contactID = contactRepository.NextID();

                var @event = new ContactCreated(contactID, 1, "Giorgio");
                var contact = new Contact(contactID);
                contact.Apply(@event);

                //contactRepository.Save(contact);
                //mediator.Publish(@event);

                return Task.FromResult(true);
            }
        }
    }
}
