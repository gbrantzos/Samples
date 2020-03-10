using System.Threading.Tasks;
using ContactManager.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IMediator mediator;

        public ContactController(IMediator mediator)
            => this.mediator = mediator;

        [HttpPost]
        public async Task<bool> CreateContact(CreateContact.Command command)
            => await mediator.Send(command);
    }
}
