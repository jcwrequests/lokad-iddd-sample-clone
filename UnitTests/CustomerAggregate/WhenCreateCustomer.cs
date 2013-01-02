using System;
using NUnit.Framework;
using Sample.Domain;

namespace Sample.CustomerAggregate
{
    /// <summary>
    /// Given-when-then unit tests for <see cref="Customer.Create"/>.
    /// See Readme file in folders above for explanations or
    /// 'framework.cs' for the testing infrastructure.
    /// </summary>
    public class WhenCreateCustomer : customer_specs
    {
        [Test]
        public void given_new_customer_and_bonus()
        {
            // dependencies
            var pricing = new TestPricingService(17m);

            // call
            When = customer => customer.Create(
                new CustomerId(1),
                "Lokad",
                Currency.Eur, pricing, new DateTime(2012, 07, 16));

            // expectations
            Then = new IEvent[]
                {
                    new CustomerCreated
                        {
                            Currency = Currency.Eur,
                            Id = new CustomerId(1),
                            Name = "Lokad",
                            Created = new DateTime(2012, 07, 16)
                        },
                    new CustomerPaymentAdded
                        {
                            Id = new CustomerId(1),
                            NewBalance = 17m.Eur(),
                            Transaction = 1,
                            Payment = 17m.Eur(),
                            PaymentName = "Welcome bonus",
                            TimeUtc = new DateTime(2012, 07, 16)
                        }
                };
        }

        [Test]
        public void given_new_customer_and_no_bonus()
        {
            // dependencies
            var pricing = new TestPricingService(0);

            // call
            When = customer => customer.Create(
                new CustomerId(1),
                "Lokad",
                Currency.Rur, pricing, new DateTime(2012, 07, 16));

            // expectations
            Then = new IEvent[]
                {
                    new CustomerCreated
                        {
                            Currency = Currency.Rur,
                            Id = new CustomerId(1),
                            Name = "Lokad",
                            Created = new DateTime(2012, 07, 16)
                        },
                };
        }
    }
}