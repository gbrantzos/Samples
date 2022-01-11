using System.Threading.Tasks;
using ContactManager.Application.Commands;
using ContactManager.Application.Queries;
using ContactManager.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/xml", "application/json")]
    public class ContactController : ControllerBase
    {
        private readonly IMediator mediator;

        public ContactController(IMediator mediator)
            => this.mediator = mediator;

        [HttpGet("{id}")]
        public async Task<ContactViewModel> Get(long id)
            => await mediator.Send(new GetContact.Query { ID = id });

        [HttpPost]
        public async Task<bool> CreateContact(CreateContact.Command command)
            => await mediator.Send(command);
    }
}
