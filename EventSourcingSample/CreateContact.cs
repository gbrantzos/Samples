using MediatR;

namespace EventSourcingSample
{
    public class CreateContact : IRequest<bool>
    {
        public string Fullname { get; set; }
    }
}
