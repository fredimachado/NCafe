using Aspirant.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var eventStore = builder.AddEventStore("eventstore")
    .WithHealthCheck()
    .WithDataVolume();

var rabbitMq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume()
    .WithHealthCheck()
    .WithManagementPlugin();

builder.AddProject<Projects.NCafe_Admin_Api>("admin-api")
    .WithReference(eventStore)
    .WaitFor(eventStore);

var cashierProject = builder.AddProject<Projects.NCafe_Cashier_Api>("cashier-api")
    .WithReference(eventStore)
    .WithReference(rabbitMq)
    .WaitFor(eventStore)
    .WaitFor(rabbitMq);

builder.AddProject<Projects.NCafe_Barista_Api>("barista-api")
    .WithReference(eventStore)
    .WithReference(rabbitMq)
    .WaitFor(cashierProject);

builder.AddProject<Projects.NCafe_Web>("web-ui")
    .WithExternalHttpEndpoints();

builder.Build().Run();
