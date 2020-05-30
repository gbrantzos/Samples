//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reactive.Concurrency;
//using System.Reactive.Linq;
//using System.Threading.Tasks;

//namespace BlueBrown.Loyalty.Journeys.Application.Processing
//{
//    public class ActivityProcessor : IActivityProcessor
//    {
//        private readonly IMediator _mediator;
//        private readonly IJourneyRepository _journeyRepository;
//        private readonly IJourneyRegistry _journeyRegistry;

//        public ActivityProcessor(IMediator mediator, IJourneyRepository journeyRepository, IJourneyRegistry journeyRegistry, int concurrency = 1)
//        {
//            this._mediator = mediator;
//            this._journeyRepository = journeyRepository;
//            this._journeyRegistry = journeyRegistry;

//            for (int consumer = 0; consumer < concurrency; consumer++)
//            {
//                int thisConsumer = consumer;

//                ActivityHub.Items
//                .Where(item => item.JourneyId % concurrency == thisConsumer)
//                .Select(item => Observable.FromAsync(async () =>
//                {
//                    try
//                    {
//                        await _mediator.Send(new ProcessActivity(item.ProcessItemId, item.JourneyId, item.Activity));
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine(ex.Message);
//                        Console.WriteLine(ex.StackTrace);
//                    }
//                }))
//                .Concat()
//                .Subscribe();
//            }
//        }

//        public async Task Handle(IReadOnlyCollection<ActivityMessage> messages)
//        {
//            var processItems =
//            messages
//            .Select(message => new
//            {
//                Message = message,
//                ActivityDescriptor = message.Activity.GetActivityDescriptor()
//            })
//            .SelectMany(
//            entry => _journeyRegistry.GetActiveJouneys(entry.Message.CustomerId, entry.ActivityDescriptor),
//            (entry, journeyId) => new ProcessActivityItem
//            {
//                ProcessItemId = Guid.NewGuid(),
//                JourneyId = journeyId,
//                Activity = entry.Message.Activity
//            })
//            .ToArray();

//            await _journeyRepository.AddProcessActivityItems(processItems);

//            foreach (var item in processItems)
//                ActivityHub.Enqueue(item);
//        }
//    }
//}