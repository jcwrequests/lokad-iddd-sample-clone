using System;
using NUnit.Framework;
using Sample.Domain;

namespace Sample.CustomerAggregate
{
    /// <summary>
    /// Given-when-then unit tests for <see cref="Customer.Rename"/>.
    /// See Readme file in folders above for explanations or 
    /// 'framework.cs' for the testing infrastructure.
    /// </summary>
    public class WhenRenameCustomer : customer_specs
    {
        [Test]
        public void given_matching_name()
        {
            Given = new IEvent[]
                {
                    new CustomerCreated
                        {
                            Id = new CustomerId(1),
                            Currency = Currency.Eur,
                            Name = "Lokad"
                        }
                };
            When = c => c.Rename("Lokad", DateTime.UtcNow);
            Then = NoEvents;
        }

        [Test]
        public void given_different_name()
        {
            Given = new IEvent[]
                {
                    new CustomerCreated
                        {
                            Id = new CustomerId(1),
                            Currency = Currency.Eur,
                            Name = "Lokad"
                        }
                };

            When = c => c.Rename("Lokad SAS", new DateTime(2012, 07, 16));
            Then = new IEvent[]
                {
                    new CustomerRenamed
                        {
                            Id = new CustomerId(1),
                            Name = "Lokad SAS",
                            OldName = "Lokad",
                            Renamed = new DateTime(2012, 07, 16)
                        }
                };
        }
    }
}