using System;
using System.Collections.Generic;

namespace Sample.Domain
{
    /// <summary>
    /// <para>Implementation of customer aggregate. In production it is loaded and 
    /// operated by an <see cref="CustomerApplicationService"/>, which loads it from
    /// the event storage and calls appropriate methods, passing needed arguments in.</para>
    /// <para>In test environments (e.g. in unit tests), this aggregate can be
    /// instantiated directly.</para>
    /// </summary>
    public class Customer
    {
        /// <summary> List of uncommitted changes </summary>
        public readonly IList<IEvent> Changes = new List<IEvent>();
        /// <summary>
        /// Aggregate state, which is separate from this class in order to ensure,
        /// that we modify it ONLY by passing events.
        /// </summary>
        readonly CustomerState _state;

        public Customer(IEnumerable<IEvent> events)
        {
            _state = new CustomerState(events);
        }

        void Apply(IEvent e)
        {
            // pass each event to modify current in-memory state
            _state.Mutate(e);
            // append event to change list for further persistence
            Changes.Add(e);
        }


        public void Create(CustomerId id, string name, Currency currency, IPricingService service, DateTime utc)
        {
            if (_state.Created)
                throw new InvalidOperationException("Customer was already created");
            Apply(new CustomerCreated
                {
                    Created = utc,
                    Name = name,
                    Id = id,
                    Currency = currency
                });

            var bonus = service.GetWelcomeBonus(currency);
            if (bonus.Amount > 0)
                AddPayment("Welcome bonus", bonus, utc);
        }
        public void Rename(string name, DateTime dateTime)
        {
            if (_state.Name == name)
                return;
            Apply(new CustomerRenamed
                {
                    Name = name,
                    Id = _state.Id,
                    OldName = _state.Name,
                    Renamed = dateTime
                });
        }

        public void LockCustomer(string reason)
        {
            if (_state.ConsumptionLocked)
                return;
            
            Apply(new CustomerLocked
                {
                    Id = _state.Id,
                    Reason = reason
                });
        }

        public void LockForAccountOverdraft(string comment, IPricingService service)
        {
            if (_state.ManualBilling) return;
            var threshold = service.GetOverdraftThreshold(_state.Currency);
            if (_state.Balance < threshold)
            {
                LockCustomer("Overdraft. " + comment);
            }

        }

        public void AddPayment(string name, CurrencyAmount amount, DateTime utc)
        {
            Apply(new CustomerPaymentAdded()
                {
                    Id = _state.Id,
                    Payment = amount,
                    NewBalance = _state.Balance + amount,
                    PaymentName = name,
                    Transaction = _state.MaxTransactionId + 1,
                    TimeUtc = utc
                });
        }

        public void Charge(string name, CurrencyAmount amount, DateTime time)
        {
            if (_state.Currency == Currency.None)
                throw new InvalidOperationException("Customer currency was not assigned!");
            Apply(new CustomerChargeAdded()
                {
                    Id = _state.Id,
                    Charge = amount,
                    NewBalance = _state.Balance - amount,
                    ChargeName = name,
                    Transaction = _state.MaxTransactionId + 1,
                    TimeUtc = time
                });
        }
    }
}