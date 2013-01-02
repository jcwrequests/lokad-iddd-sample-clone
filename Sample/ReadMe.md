This "RunMe.exe" project contains sample framework, infrastructure and 
actual domain implementation for Lokad-IDDD-Sample, which focuses solely
on A+ES concepts (Aggregates implemented with Event Sourcing) as described
in "Implementing Domain-Driven Design" by Vaughn Vernon. 

Please see README.Markdown, in the upper folder, if you need high-level overview.

You can run this project by starting in in Visual Studio (it is a console)

There are following folders:

* Domain - actual aggregates which capture core business concepts
* Projection - sample source code for event handlers which "project" events
  into persistent read models called views.
* Storage - interfaces for event store and actual implementations for various
  persistence options.

Plus, there also is UnitTests project next to this one.
