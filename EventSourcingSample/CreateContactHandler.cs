using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace EventSourcingSample
{
    public class CreateContactHandler : IRequestHandler<CreateContact, bool>
    {
        private readonly IMediator mediator;
        private readonly IContactRepository contactRepository;

        public CreateContactHandler(IMediator mediator, IContactRepository contactRepository)
        {
            this.mediator = mediator;
            this.contactRepository = contactRepository;
        }

        public Task<bool> Handle(CreateContact request, CancellationToken cancellationToken)
        {
            var contact = new Contact(request.Fullname);
            var @event  = new ContactCreated(contact.Id, contact.Fullname);

            contactRepository.Save(contact);
            mediator.Publish(@event);

            return Task.FromResult(true);
        }
    }
}
