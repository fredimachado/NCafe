using MediatR;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using NCafe.Barista.Api.Hubs;
using NCafe.Barista.Api.Projections;
using NCafe.Barista.Domain.Commands;
using NCafe.Barista.Domain.Messages;
using NCafe.Barista.Domain.Queries;
using NCafe.Barista.Domain.ReadModels;
using NCafe.Infrastructure;
using NCafe.Shared.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRabbitMQClient("rabbitmq", configureConnectionFactory: config =>
{
    config.DispatchConsumersAsync = true;
});

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddEventStoreProjectionService<BaristaOrder>(builder.Configuration)
                .AddInMemoryReadModelRepository<BaristaOrder>()
                .AddHostedService<BaristaOrderProjectionService>();

builder.Services.AddRabbitMqConsumerService(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<PlaceOrder>());

builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen();

var corsPolicyName = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName,
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

var app = builder.Build();

app.UseMessageSubscriber()
   .Subscribe<OrderPlacedMessage>(async (serviceProvider, message) =>
   {
       var mediator = serviceProvider.GetRequiredService<IMediator>();
       var hubContext = serviceProvider.GetRequiredService<IHubContext<OrderHub>>();

       // Dispatch domain command
       await mediator.Send(new PlaceOrder(
           message.Id,
           message.OrderItems.Select(i => new NCafe.Barista.Domain.Commands.OrderItem(i.ProductId, i.Name, i.Quantity, i.Price)).ToArray(),
           message.CustomerName));

       // Notify clients
       await hubContext.Clients.All.SendAsync(
           "ReceiveOrder",
           new Order(message.Id, message.OrderItems.Select(i => new NCafe.Shared.Hubs.OrderItem(i.Name, i.Quantity)).ToArray(), message.CustomerName));
   });

app.MapDefaultEndpoints();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyName);

app.MapGet("/orders", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetOrders());
    return Results.Ok(result);
})
.WithName("GetOrders");

app.MapPost("/orders/prepared", async (IMediator mediator, CompleteOrder command) =>
{
    await mediator.Send(command);
    return Results.Created("/orders", null);
})
.WithName("CompletePreparation");

app.MapHub<OrderHub>("/orderHub");

app.MapGet("/healthz", () => "OK");

app.Run();
