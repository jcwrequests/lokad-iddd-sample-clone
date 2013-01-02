using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Sample.Storage;

namespace Sample.Projections
{
    delegate void EventIndexUpdatedHandler(long lastEventIndex);

    public class DomainEventPublisher
    {
        public event EventIndexUpdatedHandler EventIndexUpdated;
        private Dictionary<Type, List<object>> projections = new Dictionary<Type,List<object>>();
        private IEventStore eventStore;
        private long lastEventIndex;
        private System.Timers.Timer polling;
        private int numberOfItemsToProcess=  0;
        private bool stopPolling = false;
        public DomainEventPublisher(IEventStore eventStore,
                                    long lastEventIndex,
                                    double pollInterval)
        {
            if (eventStore == null) throw new ArgumentNullException("eventStore");
            this.eventStore = eventStore;
            this.lastEventIndex = lastEventIndex;
            polling = new System.Timers.Timer(pollInterval);
            this.eventStore.NewEventsArrived += (int count) => IncrementItemCount(count);
            this.polling.Elapsed += (s, e) =>
            {
                if (numberOfItemsToProcess > 0)
                {
                    polling.Stop();
                    ProcessNewEvents();
                    
                    if (!stopPolling) polling.Start();

                };
            };

        }
        public void RegisterProjection<TProjection>(TProjection projection) where TProjection : class
        {
            var @events = typeof(TProjection).GetMethods().
                          Where(m => m.Name.Equals("when", StringComparison.InvariantCultureIgnoreCase)).
                          Where(m => m.GetParameters().Count().Equals(1)).
                          Select(m => m.GetParameters().First().ParameterType);

            foreach (var @event in @events)
            {
                Debug.WriteLine(@event);
                if (!projections.ContainsKey(@event))
                    projections.Add(@event,new List<object>());

                projections[@event].Add(projection);

            }
        }
        private void IncrementItemCount(int count)
        {
            System.Threading.Interlocked.Add(ref numberOfItemsToProcess,count);
        } 
        private void DecrementItemCount(int count)
        {
            System.Threading.Interlocked.Add(ref numberOfItemsToProcess, count * -1);
        }
        public void ProcessNewEvents()
        {
            while (true)
            {
                var events = eventStore.LoadEvents(lastEventIndex, numberOfItemsToProcess);
                foreach (IEvent @event in events)
                {
                    if (projections.ContainsKey(@event.GetType()))
                    {
                        foreach (var projection in projections[@event.GetType()])
                        {
                            ((dynamic)projection).When((dynamic)@event);
                        }
                    }
                }
                DecrementItemCount(events.Count());

                System.Threading.Interlocked.Add(ref lastEventIndex, (long)events.Count());
                
                if (EventIndexUpdated != null) EventIndexUpdated(lastEventIndex);

                if (numberOfItemsToProcess.Equals(0)) break;
            }
        }
        public void StartPolling()
        {
            stopPolling = false;
            polling.Start();
        }
        public void StopPolling()
        {
            stopPolling = true;
            polling.Stop();
        }
    }
}
