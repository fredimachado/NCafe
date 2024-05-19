using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using NCafe.Barista.Api.Hubs;
using NCafe.Barista.Api.Projections;
using NCafe.Barista.Domain.Commands;
using NCafe.Barista.Domain.Queries;
using NCafe.Barista.Domain.ReadModels;
using NCafe.Core.Commands;
using NCafe.Core.MessageBus.Events;
using NCafe.Core.Queries;
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
                .AddCommandHandlers<PlaceOrder>()
                .AddCommandHandlerLogger()
                .AddQueryHandlers<PlaceOrder>();

builder.Services.AddInMemoryReadModelRepository<BaristaOrder>()
                .AddEventStoreProjectionService<BaristaOrder>()
                .AddHostedService<BaristaOrderProjectionService>();

builder.Services.AddRabbitMqConsumerService(builder.Configuration);

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
   .Subscribe<OrderPlaced>(async (serviceProvider, message) =>
   {
       var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
       var hubContext = serviceProvider.GetRequiredService<IHubContext<OrderHub>>();

       // Dispatch domain command
       await commandDispatcher.DispatchAsync(new PlaceOrder(message.Id, message.ProductId, message.Quantity));

       // Notify clients
       await hubContext.Clients.All.SendAsync(
           "ReceiveOrder",
           new Order(message.Id, message.ProductId, message.Quantity));
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

app.MapGet("/orders", async (IQueryDispatcher queryDispatcher) =>
{
    var result = await queryDispatcher.QueryAsync(new GetOrders());
    return Results.Ok(result);
})
.WithName("GetOrders");

app.MapPost("/orders/{id:guid}/prepared", async (ICommandDispatcher commandDispatcher, Guid id) =>
{
    await commandDispatcher.DispatchAsync(new CompleteOrder(id));
    return Results.Created("/orders", null);
})
.WithName("CompletePreparation");

app.MapHub<OrderHub>("/orderHub");

app.MapGet("/healthz", () => "OK");

app.Run();
