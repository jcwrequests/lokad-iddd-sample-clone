using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Projections;
using Sample.Storage;
using Sample;
using Sample.Domain;
using Sample.Storage.Files;
using System.Collections.Generic;

namespace DomainTests
{
    [TestClass]
    public class ProjectionsTest
    {
        [TestMethod]
        public void REGISTER_PROJECTIONS()
        {
            var appendStore = new FileAppendOnlyStore( @"C:\Users\wydev\Documents\GitHub\Clones\lokad-iddd-sample\DomainTests\Store\");
            var eventStore = new EventStore(appendStore);
            var publisher = new DomainEventPublisher(eventStore,0,500);
            var store = new FileDocumentReaderWriter<CustomerId,CustomerTransactions>(@"C:\Users\wydev\Documents\GitHub\Clones\lokad-iddd-sample\DomainTests\Store\",
                                                                                new ViewStrategy(@"C:\Users\wydev\Documents\GitHub\Clones\lokad-iddd-sample\DomainTests\Views\"));
            IDocumentWriter<CustomerId,CustomerTransactions> writer = store;

            var projection = new CustomerTransactionsProjection(writer);

            publisher.RegisterProjection(projection);
            var id = new CustomerId(2);
            var @event = new CustomerCreated
                        {
                            Id = id,
                            Name = "Microsoft", 
                            Currency = Currency.Eur
                        };
            
            IList<IEvent> Changes = new List<IEvent>();
            Changes.Add(@event);

            eventStore.AppendToStream(id,0,Changes);

            publisher.ProcessNewEvents();


        }
    }
}
