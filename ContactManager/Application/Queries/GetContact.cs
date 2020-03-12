using System.Threading;
using System.Threading.Tasks;
using ContactManager.Application.ViewModels;
using ContactManager.Domain.Model;
using MediatR;

namespace ContactManager.Application.Queries
{
    public static class GetContact
    {
        public class Query : IRequest<ContactViewModel>
        {
            public long ID { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, ContactViewModel>
        {
            private readonly IContactRepository contactRepository;

            public QueryHandler(IContactRepository contactRepository)
                => this.contactRepository = contactRepository;

            public async Task<ContactViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                var contact = await contactRepository.GetByID(request.ID);
                return new ContactViewModel
                {
                    ID = contact.ID,
                    FullName = contact.FullName
                };
            }
        }
    }
}
