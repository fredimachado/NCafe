using NCafe.Abstractions.Commands;
using NCafe.Abstractions.Queries;
using NCafe.Barista.Api.MessageBus;
using NCafe.Barista.Api.Projections;
using NCafe.Barista.Domain.Commands;
using NCafe.Barista.Domain.Queries;
using NCafe.Barista.Domain.ReadModels;
using NCafe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddCommandHandlers(typeof(PlaceOrder).Assembly)
                .AddCommandHandlerLogger()
                .AddQueryHandlers(typeof(PlaceOrder).Assembly);

builder.Services.AddInMemoryReadModelRepository<BaristaOrder>()
                .AddHostedService<OrderProjectionService>();

builder.Services.AddHostedService<OrdersConsumer>();

builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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

app.Run();
