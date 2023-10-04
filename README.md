# <img src="images/logo.svg?raw=true" alt="Logo" width="28" /> NCafe

Minimal .NET microservices implementation in the context of a cafe.

Heavily inspired on the [microcafe](https://github.com/rbanks54/microcafe) project by [Richard Banks](https://github.com/rbanks54).

More information about the application domain, please check the links below:

https://www.slideshare.net/rbanks54/architecting-microservices-in-net
https://www.enterpriseintegrationpatterns.com/ramblings/18_starbucks.html

### Tech stack:

- **C# 11/.NET 7**
- **ASP.NET Core minimal APIs**
- **Docker**
- **Kubernetes**
- **EventStore**: Database for Event Sourcing where we store events as the source of truth instead of current state
- **RabbitMQ**: Message broker used for asynchronous messaging

### Warning

This code should be treated as sample code. It's a sandbox for practicing microservice ideas,
CQRS, Event Sourcing, Kubernetes/helm deployment and related things.

I wrote some documentation below for those who are starting on this journey. I hope it helps! :smile:

### TODO

- [x] Use SignalR to update the Barista page in real time as new orders are placed
- [x] Kubernetes/helm/helmfile deployment to my internal K3S cluster
- [ ] Add observability with OpenTelemetry
- [ ] Use a database for the read models (projections)
- [ ] Move projection services to their own microservice so reads and writes are separate
- [ ] Add option for cashier to mark order as paid (only then ready for barista) or cancel it
- [ ] Cashier should be able to add the customer name to the order
- [ ] Cashier should be able to add multiple products to the same order
- [ ] Barista should be able to see the name of the customer and products/quantities
- [ ] Add timestamp to orders so we can show the "oldest" first
- [ ] Admin should be able to edit products
- [ ] Admin should be able to delete products

## Content

- [NCafe Solution](#ncafe-solution)
  - [Core (Shared Abstractions)](#core-shared-abstractions)
  - [Application Domain](#application-domain)
  - [Web API](#web-api)
  - [Infrastructure](#infrastructure)
- [NCafe's CQRS + Event Sourcing implementation](#ncafes-cqrs-event-sourcing-implementation)
  - [Command](#command)
  - [Query](#query)
  - [Projections](#projections)
- [How to run](#how-to-run)
  - [Starting the infrastructure containers](#starting-the-infrastructure-containers)
  - [Starting the microservices](#starting-the-microservices)
  - [Swagger](#swagger)
  - [NCafe in action](#ncafe-in-action)
  - [EventStore](#eventstore)
  - [RabbitMQ](#rabbitmq)
  - [Stopping everything](#stopping-everything)

## NCafe Solution

The NCafe Solution is based on Clean Architecture, so dependencies only point torwards the center.

![Clean Architecture](images/architecture.png?raw=true)

#### Core (Shared Abstractions)

All common interfaces and abstractions are here. For example:

- **AggregateRoot**: Abstract base class for our domain entities
- **IEvent and Event**: Base for events that represent state changes of entities
- **IRepository**: Interface used to fetch and save domain entities
- **IPublisher**: Interface for publishing integration events
- **Shared events**: Events that will be published with IPublisher
- **Read Model**: Interface used to fetch and save read models (projections)
- **Command, Query, Dispatchers and Handlers interfaces**
- **Basic exceptions**

So, basically, this project is the core of our solution and will only contain the
abstractions used by other layers. This project doesn't have any microservice specific code.

#### Shared

There is a shared project (`NCafe.Shared`), that contains code that don't require any core
abstraction or logic. For example, types that define SignalR objects for real-time functionality.

The Web UI project doesn't really need a reference to `NCafe.Core` (`NCafe.Shared` is enough).

### Application Domain

Depends only on the Core project in order to define domain entities (aggregates),
events, commands, queries, their respective handlers and business logic. So it might have:

- **Entities**: Their current state is defined by a stream of events stored in EventStore.
Provide methods to make it do something (ex.: `CompletePreparation`).
These methods will raise events (after doing some validation if necessary), that will be appended
to its event stream in EventStore
- **Events**: Represent something that has happened in the domain (ex.: `OrderPlaced`). These events
will be raised by the entities as described above
- **Commands/Handlers**: Commands and handlers will perform some kind of action. Most probably
changing the state of an entity. Must use
abstractions/interfaces (ex.: `IRepository`) instead of implementations
- **Queries/Handlers**: Used to return data from read models. Must use abstractions/interfaces
(ex.: `IReadModelRepository`) instead of implementations
- **Read Models**: To be used by projection services and query handlers

### Web API

The Web API projects simply register the required dependencies using methods from the Infrastructure project and map the
endpoints, which use `ICommandDispatcher` or `IQueryDispatcher` (see `Program.cs`).

These projects have a reference to its Application Domain project and the Infrastructure Project.

Projection services can also be in the Web API project (Find more about projections below).

In case the microservice needs to consume integration events, a Consumer service can be created
(see `OrdersConsumerService` in `Barista.Api`). Basically, this service implements .NET's `IHostedService`, subscribes
to a RabbitMQ stream specifying a queue, a topic and a callback, which in case will use `ICommandDispatcher` to, you guessed it,
dispatch a command to the domain.

### Infrastructure

Implementations for external dependencies are defined in this project. Like:

- EventStore Repository and Projection Service
- RabbitMQ publisher

There are some other implementations in here as well, like Command and Query dispatchers, a Logging decorator and
read model repositories (only in-memory for now).

This project also contains methods for registering all the implementations for interfaces defined
in the Core project.

## NCafe's CQRS + Event Sourcing implementation

![CQRS and Event Sourcing in NCafe](images/ncafe-cqrs-event-sourcing.png?raw=true)

Inspired by https://codeopinion.com/cqrs-event-sourcing-code-walk-through/.

### Command

Depending on the command, the handler will instantiate the domain model (entity/aggregate), for example in `PlaceOrderHandler`.
Then the entity will be saved using `IRepository`.

For other commands, the handler will first fetch the entity by id from the repository,
then tell it to do something (ex.: `CompletePreparation`) and then save it using the repository.

When fetching an entity, the repository will actually get all events for the specific aggregate and apply them in order. This will
re-build the current state of the entity based on the events.

The Save method in the repository sends pending events to EventStore, for example, the `OrderPrepared` event
after `CompletePreparation` is called (see the Barista.Domain project).

### Query

This is the simplest part of the system. The handlers will use a read model repository to return one or more items from the
query database (in-memory for now). The data from this database comes from a projection service, described below.

### Projections

All Projection services subscribed to EventStore event streams (ex.: `baristaOrder`), will receive new events
from EventStore and use it to update the query database (read model) if necessary.

In NCafe, Projection services are implemented using .NET's `BackgroundService` running in the API projects, as the read models are
being stored using an in-memory repository for the time being. But when switch to a proper database, we can easily move the
projection specific code to it's own microservice. In fact, I think we will have to move the code, otherwise we need a way to run
only one projection service in case there's a need to scale out (create more instances of) the API microservice. In case you're
wondering why, it's because I'm imagining multiple projection services subscribed to the same streams and also using the same
database. So they would all try to do the same inserts/updates. It looks like trouble.

Also, when we move to a database, we'll need to save checkpoints, so when we restart the projection service we can tell EventStore
that we only care about events from a specific position in the stream. Saving the version should be enough.

## How to run

In order to run the solution, you need the following:

- .NET 7 SDK
- Docker

In case you want to run the microservices using docker (except the infrastructure ones), we need to generate a certificate,
but we only need to do this once though.

### Starting the infrastructure containers

Run the following command:

    docker compose -f infrastructure-compose.yaml up -d

### Starting the microservices

You can start the microservices via the dotnet CLI or your favorite IDE/code editor.
If you're using Visual Studio you can also set multiple startup projects by going to solution properties.

If you prefer docker, run the following command to build and start all microservices containers (certificate required):

    docker compose -f services-compose.yaml up -d

### Swagger

- **Admin**: [http://localhost:5010/swagger/index.html](http://localhost:5010/swagger/index.html)
- **Cashier**: [http://localhost:5020/swagger/index.html](http://localhost:5020/swagger/index.html)
- **Barista**: [http://localhost:5030/swagger/index.html](http://localhost:5030/swagger/index.html)

### NCafe in action

1. Start the NCafe.Web project (Blazor WebAssembly)
2. Create a new product in the Admin page
3. Place an order in the Cashier page
4. Complete the order in the Barista page

![Admin Screenshot](images/admin.png?raw=true)

Or, you can use Swagger for example:

1. Create a product via the POST `/products` endpoint in **Admin**
2. Get a product Id using the GET `/products` endpoint
3. Place an order via the POST `/orders` endpoint in **Cashier**
4. Get the order Id using the GET `/orders` endpoint in **Barista**
5. Complete the order via the POST `/orders/{id}/prepared` endpoint in **Barista**

### EventStore

You can check all the events in EventStore by going to the `Stream Browser`
in [http://localhost:2113/](http://localhost:2113/).

![EventStore Screenshot](images/eventstore.png?raw=true)

### RabbitMQ

You can check the message queue in [http://localhost:15672/](http://localhost:15672/).

### Stopping everything

Run the following command:

    docker compose -f services-compose.yaml -f infrastructure-compose.yaml down
