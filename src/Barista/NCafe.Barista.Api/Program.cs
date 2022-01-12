using NCafe.Abstractions.Commands;
using NCafe.Barista.Api.EventBus;
using NCafe.Barista.Domain.Commands;
using NCafe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddCommandHandlers(typeof(PlaceOrder).Assembly)
                .AddCommandHandlerLogger()
                .AddQueryHandlers(typeof(PlaceOrder).Assembly);

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


app.MapPost("/orders/{id:guid}/prepared", async (ICommandDispatcher commandDispatcher, Guid id) =>
{
    await commandDispatcher.DispatchAsync(new CompleteOrder(id));
    return Results.Created("/orders", null);
})
.WithName("CompletePreparation");

app.Run();
