# NCafe

Minimal .NET microservices implementation in the context of a cafe.

Heavily inspired on the [microcafe](https://github.com/rbanks54/microcafe) project by [Richard Banks](https://github.com/rbanks54).

### Warning

This code should be treated as sample code, not production-ready code.

## Infrastructure

NCafe microservices require the following services:

- **EventStore**: Database built for Event Sourcing where we store events as a source of truth instead of current state.
- **Kafka/Zookeeper**: Event streaming platform that NCafe uses for integration events.
- **Kafdrop**: Web UI for viewing Kafka topics and browsing consumer groups.

## How it works

I have no idea... Just kidding, I just don't want to write about it right now. More info to come...

## How to run

In order to run the solution, you need the following:

- .NET 6 SDK
- Docker

Since we're using https, we need to generate a certificate, but we only need to do this once though.

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

Once the containers have started for the first time, access Kafdrop: [http://localhost:9000](http://localhost:9000)

Click the `New` button at the bottom and create a topic named `orders` (default options should be fine).
The topic should still be there if you restart the infrastructure containers.

### Starting microservices

Run the following command to build and start all microservices containers:

    docker compose -f services-compose.yaml up -d

Of course, you can also run them via Visual Studio or VS Code.

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
