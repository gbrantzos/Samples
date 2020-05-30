//using System;
//using System.Reactive.Linq;

//namespace BlueBrown.Loyalty.Journeys.Application.Processing
//{
//    public class ProcessActivityItem
//    {
//        public Guid ProcessItemId { get; set; }
//        public long JourneyId { get; set; }
//        public Activity Activity { get; set; }
//    }

//    static class ActivityHub
//    {
//        private delegate void ActivityEnqueue(ProcessActivityItem item);

//        private static event ActivityEnqueue ActivityEnqueueEvent;

//        public static IObservable<ProcessActivityItem> Items =
//        Observable.FromEvent<ActivityEnqueue, ProcessActivityItem>(
//        handler => ActivityEnqueueEvent += handler,
//        handler => ActivityEnqueueEvent -= handler);

//        public static void Enqueue(ProcessActivityItem item)
//        {
//            ActivityEnqueueEvent?.Invoke(item);
//        }
//    }
//}