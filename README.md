Sample code for IDDD Book by Vaughn Vernon
==========================================

This is a .NET Sample Project to accompany Event Sourcing materials 
from the book by [Vaughn Vernon](http://vaughnvernon.co/): Implementing Domain-Driven Design.

Event Sourcing pattern was introduced to DDD world by [Gregory Young](http://goodenoughsoftware.net/). 
A+ES appendix in IDDD book and this sample are mainly based on his work in pushing the state of
art in this field.

### Contents

This project includes a sample domain implemented with the event sourcing pattern. 
Multiple persistence options are provided:

* Microsoft SQL Server
* MySQL
* File storage
* Windows Azure Blob Storage

### Usage

Start by [downloading sources](https://github.com/Lokad/lokad-iddd-sample/zipball/master) (ZIP, Visual Studio) 
or by cloning a [git repository](https://github.com/Lokad/lokad-iddd-sample):

```
$ git clone git://github.com/Lokad/lokad-iddd-sample.git
```


Then, open solution in Visual Studio (MonoDevelop might also work) and
run the project. The project is auto-configured to use file storage by default,
and cleans it on every start.

You should see something like:

```
Command: Create customer-12 named 'Lokad' with Eur
  customer-12 r0 Event: Customer Lokad created with Eur
  customer-12 r0 Event: Added 'Welcome bonus' 15 EUR | Tx 1 => 15 EUR
  Completed in 145 ms
Command: Rename customer-12 to 'Lokad SAS'
  customer-12 r1 Event: Customer renamed from 'Lokad' to 'Lokad SAS'
  Completed in 27 ms
Command: Charge 20 EUR - 'Forecasting'
  customer-12 r2 Event: Charged 'Forecasting' 20 EUR | Tx 2 => -5 EUR
  Completed in 16 ms
```

Then, you can dive into the code by starting with `Domain` folder in this solution.
There also is a `UnitTests` project, which contains unit tests for the domain
(in form of Given-When-Then specifications)

You can also try plugging in other types of event stores. Each event store requires 
connection string and will auto-create all required resources automatically.

### Want More?

If you want more details, here's what you can do next:

* Check out the [Domain Driven Design community](http://dddcommunity.org/)
* Dive into [Lokad.CQRS Sample Project](http://lokad.github.com/lokad-cqrs/) (much more detailed and practical version of this sample)
* Check out [CQRS/DDD Pocket Guide](http://cqrsguide.com/case-studies) and [case studies](http://cqrsguide.com/case-studies)
* Follow [@VaughnVernon](https://twitter.com/#!/VaughnVernon).
* Follow [@abdullin](https://twitter.com/#!/abdullin).

If you have any questions, please feel free to ask the contributors.

### Authors and Contributors

* [Vaughn Vernon](http://vaughnvernon.co/), Book author and reviewer.
* [Rinat Abdullin](http://abdullin.com), A+ES text and this sample project. Tech Leader at [Lokad](http://www.lokad.com/), Big Data Analytics for Retail.
* [Gregory Young](http://goodenoughsoftware.net/), theory and practice of introducing Event Sourcing to Domain-Driven Design. 

This sample is based hugely on inspiration and support provided by following outstanding gentlemen:

* [Eric Evans](https://twitter.com/#!/ericevans0) - father of Domain-Driven Design
* [Jérémie Chassaing](https://twitter.com/#!/thinkb4coding) - Event Sourcing practitioner from Paris. F# Maniac that built functional ES sample.
* [Yves Reynhout](https://twitter.com/#!/yreynhout) - DDD and ES practitioner from Belgium, with really deep understanding.
* [DDD/CQRS Google Group](https://groups.google.com/forum/?fromgroups#!forum/dddcqrs)


As for bugs in the code and typos in the accompanying texts, please feel free to attribute them to Rinat Abdullin. 
Reporting them (e.g. by dropping me a [tweet](https://twitter.com/#!/abdullin) or an email) would be extra cool :)