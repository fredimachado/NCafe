var builder = DistributedApplication.CreateBuilder(args);

var eventStore = builder.AddEventStore("eventstore");
var rabbitMq = builder.AddRabbitMQ("rabbitmq");

var adminProject = builder.AddProject<Projects.NCafe_Admin_Api>("admin-api")
    .WithReference(eventStore);

var baristaProject = builder.AddProject<Projects.NCafe_Barista_Api>("barista-api")
    .WithReference(eventStore)
    .WithReference(rabbitMq);

var cashierProject = builder.AddProject<Projects.NCafe_Cashier_Api>("cashier-api")
    .WithReference(eventStore)
    .WithReference(rabbitMq);

var webUiProject = builder.AddProject<Projects.NCafe_Web>("web-ui")
    .WithExternalHttpEndpoints();

builder.Build().Run();
