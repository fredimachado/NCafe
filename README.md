# <img src="images/logo.svg?raw=true" alt="Logo" width="28" /> NCafe

Minimal .NET microservices implementation in the context of a cafe.

Heavily inspired on the [microcafe](https://github.com/rbanks54/microcafe) project by [Richard Banks](https://github.com/rbanks54).

### Warning

This code should be treated as sample code, not production-ready code.

## Infrastructure

NCafe microservices require the following services:

- **EventStore**: Database built for Event Sourcing where we store events as a source of truth instead of current state.
- **RabbitMq**: Message broker used for asynchronous messaging.

## Architecture

NCafe's architecture is based on Clean Architecture, so dependencies only point torwards the center.
Shared abstractions in this case. Yeah, I should probably name it "Core" I guess. Sorry, naming is hard for me.

![Clean Architecture](images/architecture.png?raw=true)

#### Shared Abstractions (Core)

All common interfaces and abstractions are here. For example:

- **AggregateRoot**: Abstract base class for our domain entities
- **IEvent and Event**: Base for events that represent state changes of entities
- **IRepository**: Interface used to get and save domain entities
- **IPublisher**: Interface for publishing integration events
- **Shared events**: Events that will be published with IPublisher
- **Read Model**: Interface used to get and save read models (projections)
- **Command, Query, Dispatchers and Handlers interfaces**
- **Basic exceptions**

So, basically, no implementations are allowed in this project. It will be referenced by
application domain projects.

### Application Domain

Depends only on Shard Abstractions (Core) in order to define domain entities (aggregates),
events, commands, queries and their respective handlers and business logic. So it might have:

- **Entities**: It's current state is defined by a stream of events stored in EventStore.
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

Soon...

### Infrastructure

Soon...

## NCafe's CQRS + Event Sourcing implementation

![CQRS and Event Sourcing in NCafe](images/ncafe-cqrs-event-sourcing.png?raw=true)

### Command

Depending on the command, the handler will instantiate the domain model (entity/aggregate), for example in `PlaceOrderHandler`.
Then the entity will be saved using `IRepository`.

For other commands, the handler will first fetch the entity from the repository
by Id, then tell it to do something (ex.: `CompletePreparation`) and then save it using the repository.

When fetching an entity, the repository will actually get all events for the specific aggregate and apply them in order. This will
re-build the current state of the entity based on the events.

The Save method in the repository actually sends pending events to EventStore, for example, the `OrderPrepared` event
after `CompletePreparation` is called (see the Barista.Domain project).

### Projections

All Projection services subscribed to the affected EventStore event stream (ex.: `baristaOrder`), will receive the new event
from EventStore and use it to update the query database (read model) if necessary.

In NCafe, Projection services are implemented using .NET's BackgroundService running in the API projects, as the read models are
being stored using an in-memory repository for the time being. But when switch to a proper database, we can easily move the
projection specific code to it's own microservice. In fact, I think we will have to move the code, otherwise we need a way to run
only one projection service in case there's a need to scale out (create more instances of) the API microservice. In case you're
wondering why, it's because I'm imagining multiple projection services subscribed to the same streams and also using the same
database. So they would all try to do the same inserts/updates. It looks like trouble.

Also, when we move to a database, we'll need to save checkpoints, so when we restart the projection service we can tell EventStore
that we only care about events from a specific position in the stream. Saving the version should be enough.

### Query

This is the simplest part of the system. The handlers will use a read model repository to return one or more items from the
query database (in-memory for now). The data from this database comes from a projection service, described above.

## How to run

In order to run the solution, you need the following:

- .NET 6 SDK
- Docker

In case you want to run the microservices in docker (except the infrastructure ones), we need to generate a certificate,
but we only need to do this once though.

You can skip this if you will run the microservices via the dotnet CLI or an IDE/code editor.

### If you're running Windows using Linux containers

Run the following commands on your favorite terminal:

    dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\ncafe-aspnetapp.pfx -p ncafe
    dotnet dev-certs https --trust

I'm assuming you're using PowerShell. If you're using CMD, replace `$env:USERPROFILE` with `%USERPROFILE%`.
If you decide to use a different Pfx file name or password, you'll have to update `.env-local` accordingly.

### If you're on macOS or Linux

Run the following commands:

    dotnet dev-certs https -ep ${HOME}/.aspnet/https/ncafe-aspnetapp.pfx -p ncafe
    dotnet dev-certs https --trust

More info about hosting ASP.NET Core images with Docker Compose over HTTPS: https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-6.0

### Starting the infrastructure containers

Run the following command:

    docker compose -f infrastructure-compose.yaml up -d

### Starting microservices

You can start the microservices via the dotnet CLI or your favorite IDE/code editor.

If you prefer docker, run the following command to build and start all microservices containers (certificate required):

    docker compose -f services-compose.yaml up -d

### Swagger

- **Admin**: [https://localhost:5010/swagger/index.html](https://localhost:5010/swagger/index.html)
- **Cashier**: [https://localhost:5020/swagger/index.html](https://localhost:5020/swagger/index.html)
- **Barista**: [https://localhost:5030/swagger/index.html](https://localhost:5030/swagger/index.html)

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

### Stopping everything

Run the following command:

    docker compose -f services-compose.yaml -f infrastructure-compose.yaml down
