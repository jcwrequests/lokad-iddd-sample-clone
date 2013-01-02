using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sample.Storage
{
    /// <summary>
    /// Actual implementation of <see cref="IEventStore"/>, which deals with
    /// serialization and naming in order to provide bridge between event-centric
    /// domain code and byte-based append-only persistence
    /// </summary>
    public class EventStore : IEventStore
    {
        readonly BinaryFormatter _formatter = new BinaryFormatter();

        byte[] SerializeEvent(IEvent[] e)
        {
            using (var mem = new MemoryStream())
            {
                _formatter.Serialize(mem, e);
                return mem.ToArray();
            }
        }

        IEvent[] DeserializeEvent(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            {
                return (IEvent[])_formatter.Deserialize(mem);
            }
        }

        public EventStore(IAppendOnlyStore appendOnlyStore)
        {
            _appendOnlyStore = appendOnlyStore;
        }

        readonly IAppendOnlyStore _appendOnlyStore;
        public EventStream LoadEventStream(IIdentity id, long skip, int take)
        {
            var name = IdentityToString(id);
            var records = _appendOnlyStore.ReadRecords(name, skip, take).ToList();
            var stream = new EventStream();

            foreach (var tapeRecord in records)
            {
                stream.Events.AddRange(DeserializeEvent(tapeRecord.Data));
                stream.Version = tapeRecord.Version;
            }
            return stream;
        }
        public IList<IEvent> LoadEvents(long skip, int take)
        {
            var records = _appendOnlyStore.ReadRecords(skip, take).ToList();
            var events = new List<IEvent>();

            foreach (var tapeRecord in records)
            {
                events.AddRange(DeserializeEvent(tapeRecord.Data));
            }
            return events;
        }
        string IdentityToString(IIdentity id)
        {
            // in this project all identities produce proper name
            return id.ToString();
        }

        public EventStream LoadEventStream(IIdentity id)
        {
            return LoadEventStream(id, 0, int.MaxValue);
        }

        public void AppendToStream(IIdentity id, long originalVersion, ICollection<IEvent> events)
        {
            if (events.Count == 0)
                return;
            var name = IdentityToString(id);
            var data = SerializeEvent(events.ToArray());
            try
            {
                _appendOnlyStore.Append(name, data, originalVersion);
                if (NewEventsArrived != null) NewEventsArrived(events.Count);
            }
            catch(AppendOnlyStoreConcurrencyException e)
            {
                // load server events
                var server = LoadEventStream(id, 0, int.MaxValue);
                // throw a real problem
                throw OptimisticConcurrencyException.Create(server.Version, e.ExpectedStreamVersion, id, server.Events);
            }

            // technically there should be a parallel process that queries new changes 
            // from the event store and sends them via messages (avoiding 2PC problem). 
            // however, for demo purposes, we'll just send them to the console from here
            //Console.ForegroundColor = ConsoleColor.DarkGreen;
            //foreach (var @event in events)
            //{
            //    Console.WriteLine("  {0} r{1} Event: {2}", id,originalVersion, @event);
            //}
            //Console.ForegroundColor = ConsoleColor.DarkGray;
        }


        public event NewEventsArrivedHandler NewEventsArrived;
    }
}