using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Sandbox
{
    public class WorkItem<TPayload>
    {
        public Guid ID { get; }
        public DateTime CreatedAt { get; }

        public TPayload Payload { get; }

        public WorkItem(TPayload payload)
        {
            this.ID = Guid.NewGuid();
            this.CreatedAt = DateTime.UtcNow;

            this.Payload = payload;
        }
    }

    public abstract class WorkItemProcessor<TPayload>
    {
        private static class WorkItemQueue<T>
        {
            // Worker delegate
            public delegate void WorkItemHandler(WorkItem<T> workItem);

            // Queue event
            private static event WorkItemHandler WorkItemHandlerEvent;

            // Work items Observable
            public static IObservable<WorkItem<T>> Items = Observable.FromEvent<WorkItemHandler, WorkItem<T>>(
                handler => WorkItemHandlerEvent += handler,
                handler => WorkItemHandlerEvent -= handler);

            // Add work item on queue
            public static void AddItem(WorkItem<T> workItem) => WorkItemHandlerEvent?.Invoke(workItem);
        }

        protected int Workers { get; }

        protected WorkItemProcessor(int workers = 1)
        {
            this.Workers = workers;
            for (int j = 0; j < workers; j++)
            {
                int currentWorker = j;
                WorkItemQueue<TPayload>.Items
                    .Where((item, index) =>
                    {
                        var workerIndex = WorkerSelector(item, index);
                        if (workerIndex < 0 || workerIndex >= Workers)
                            throw new ArgumentOutOfRangeException(nameof(WorkerSelector));
                        return workerIndex == currentWorker;
                    })
                    .Select(item => Observable.FromAsync(() => Process(item, currentWorker)))
                    .Concat()
                    .Subscribe();
            }
        }

        /// <summary>
        /// The "actual" processing of <see cref="WorkItem{TPayload}"/> item
        /// </summary>
        /// <param name="workItem">Work item</param>
        /// <param name="worker">Index of executing worker</param>
        /// <returns></returns>
        protected abstract Task Process(WorkItem<TPayload> workItem, int worker);

        /// <summary>
        /// Worker selection method. Result must be between 0 and (less than) total Workers
        /// </summary>
        /// <param name="workItem"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual int WorkerSelector(WorkItem<TPayload> workItem, int index) => index % this.Workers;

        #region Public methods
        /// <summary>
        /// Add <paramref name="item"/> to queue for processing
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(TPayload item) => WorkItemQueue<TPayload>.AddItem(new WorkItem<TPayload>(item));

        /// <summary>
        /// Add multiple items to queue
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<TPayload> items) { foreach (var item in items) Add(item); }
        #endregion
    }
}
