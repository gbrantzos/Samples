using System;
using System.Threading.Tasks;
using EventSourcingSample;

namespace EventSourcimgSample
{
    public static class Program
    {
        public static async Task Main()
        {
            IEventStore eventStore = new InMemoryEventStore();
            IContactRepository repository = new ContactRepository(eventStore);

            var contact = await repository.GetById(Guid.NewGuid());

            Console.WriteLine($"Hello {contact.Fullname}!");
        }
    }
}
