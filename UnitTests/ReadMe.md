> Rinat Abdullin, 2012-07-16

This project contains simple testing "framework" implementation for writing
self-documenting unit-tests for A+ES. It uses NUnit as underlying unit test.

The purpose of this implementation is to demonstrate principles of testing 
for Aggregates implemented with Event Sourcing (as described in IDDD book by
Vaughn Vernon). If you are interested in more detailed and deep implementation
of specifications - check out [Lokad.CQRS](http://lokad.github.com/lokad-cqrs/).

These unit tests are known as "specifications" or "given-when-then" tests (GWT).
Within such tests we establish that:

* given certain events;
* when a command is executed (our case);
* then we expect some specific events to happen (or an exception is thrown).

Each unit test defines a case, which not only serves as a unit test, but can also
print human-readable description, when you run it. General rule of thumb is 
to have at least one test fixture per aggregate method. This fixture will 
be named by method and will have multiple tests which verify varuous use cases.

Please, see "framework.cs" for the actual infrastructure or "when_*" classes 
for actual unit test fixtures with partial self-documenting capabilities.

If you are interested in more:

* Read about [Specifications](http://cqrsguide.com/doc:specification)
* See [Lokad.CQRS](http://lokad.github.com/lokad-cqrs/) sample project for more 
  detailed and deep implementation