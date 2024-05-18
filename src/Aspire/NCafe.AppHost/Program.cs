var builder = DistributedApplication.CreateBuilder(args);

var eventStore = builder.AddEventStore("eventstore")
    .WithHealthCheck()
    .WithDataVolume();

var rabbitMq = builder.AddRabbitMQ("rabbitmq")
    .WithHealthCheck()
    .WithManagementPlugin();

var adminProject = builder.AddProject<Projects.NCafe_Admin_Api>("admin-api")
    .WithReference(eventStore)
    .WaitFor(eventStore);

var baristaProject = builder.AddProject<Projects.NCafe_Barista_Api>("barista-api")
    .WithReference(eventStore)
    .WithReference(rabbitMq)
    .WaitFor(eventStore)
    .WaitFor(rabbitMq);

var cashierProject = builder.AddProject<Projects.NCafe_Cashier_Api>("cashier-api")
    .WithReference(eventStore)
    .WithReference(rabbitMq)
    .WaitFor(eventStore)
    .WaitFor(rabbitMq);

var webUiProject = builder.AddProject<Projects.NCafe_Web>("web-ui")
    .WithExternalHttpEndpoints();

builder.Build().Run();
