using System;
using System.Threading;
using System.Threading.Tasks;
using ContactManager.Domain.Model;
using MediatR;

namespace ContactManager.Application.EventHandlers
{
    public class ContactProjection : INotificationHandler<ContactCreated>
    {
        public async Task Handle(ContactCreated notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Projection handler => {notification.FullName}");
            await Task.CompletedTask;
            //return Task.CompletedTask;
        }
    }
}
