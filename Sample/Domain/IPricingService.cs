using System;

namespace Sample.Domain
{
    /// <summary>
    /// <para>
    /// This is a sample of domain service, that will be injected by application service
    /// into aggregate for providing this specific behavior as <see cref="PricingService"/>. 
    /// </para>
    /// <para>
    /// During tests, this service will be replaced by test implementation of the same 
    /// interface (no, you don't need mocking framework, just see the unit tests project).
    /// </para>
    /// </summary>
    public interface IPricingService
    {
        CurrencyAmount GetOverdraftThreshold(Currency currency);
        CurrencyAmount GetWelcomeBonus(Currency currency);
    }

    /// <summary>
    /// <para>This is a sample implementation of a <em>Domain Service</em> for pricing. 
    /// Such services can be more complex than that (i.e.: providing access to payment
    /// gateways, cloud fabrics, remote catalogues, expert systems or other 3rd party
    /// services). Things that involve such complex computations or remote calls can 
    /// timeout, fail or blow up. If this is expected and possible, then we can build-in 
    /// compensation logic for that. </para> 
    /// 
    /// <para>The simplest option is to put such compensation logic within the application
    /// service itself (usually inside an aggregate hosted by such app service), 
    /// wrapping actual service call inside  WaitFor (google "WaitFor Lokad github") and 
    /// various retry policies, while catching exceptions and publishing appropriate events.  
    /// </para>
    /// 
    /// <para>Check out sample of LokadRequest (from .NET client for our forecasting API) 
    /// for a sample of retries 
    /// https://github.com/Lokad/lokad-sdk/blob/master/dot-net-rest/Source/Lokad.Forecasting.Client/LokadRequest.cs
    /// </para>
    /// 
    /// <para>However this approach can complicate aggregate code by unnecessary tech details.
    /// In this case we can push integration details into a separate bounded context. This BC
    /// will simply ensure that, whenever a command is received (e.g. "ChargeCreditCard")
    /// either "CreditCardCharged" event is published or "CreditCardChargeFailed" shows up
    /// within 5 minutes (timeouts are also handled). This also works for big-data processing
    /// scenarios, where actual data manipulation is performed by a separate bounded context. 
    /// </para>
    /// <para>
    /// This approach (of explicitly modeling integration) is worth it, when integration failures
    /// are both frequent and important for your domain (e.g. you charge your customers with 
    /// the help from 3rd party gateway).
    /// </para>
    /// <para>
    /// Such separate bounded context can use remote tracker to keep an eye on the timeouts
    /// and actually publish failure events if nothing happened for too long.  
    /// http://abdullin.com/journal/2012/4/21/ddd-evolving-business-processes-a-la-lokad.html.
    /// </para>
    /// 
    /// <para>Unfortunately, this topic is a bit too big for the A+ES sample for IDDD book. 
    /// However I will try to address it within Lokad.CQRS sample project, while adding
    /// rich domain model. </para>
    /// 
    /// <para>This is written by Rinat Abdullin on 2012-07-19 at Pulkovo. If you are reading this
    /// after more than 2 months since that date, and Lokad.CQRS project still does not address
    /// this issue, please kick me in the twitter or email.</para>
    /// </summary>
    public sealed class PricingService : IPricingService
    {
        public CurrencyAmount GetOverdraftThreshold(Currency currency)
        {
            if (currency == Currency.Eur)
                return (-10m).Eur();
            throw new NotImplementedException("TODO: implement other currencies");
        }

        public CurrencyAmount GetWelcomeBonus(Currency currency)
        {
            if (currency == Currency.Eur)
                return 15m.Eur();
            throw new NotImplementedException("TODO: implement other currencies");
        }
    }

}