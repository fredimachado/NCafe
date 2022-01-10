using NCafe.Abstractions.Commands;
using NCafe.Abstractions.Queries;
using NCafe.Abstractions.ReadModels;
using NCafe.Cashier.Api.ReadModel;
using NCafe.Cashier.Application.Commands;
using NCafe.Cashier.Application.Queries;
using NCafe.Cashier.Application.ReadModels;
using NCafe.Infrastructure;
using NCafe.Infrastructure.ReadModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddCommandHandlers(typeof(PlaceOrder).Assembly)
                .AddCommandHandlerLogger()
                .AddQueryHandlers(typeof(PlaceOrder).Assembly);

builder.Services.AddSingleton<IReadModelRepository<Product>, InMemoryReadModelRepository<Product>>()
                .AddHostedService<Worker>();

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

app.MapGet("/products", async (IQueryDispatcher queryDispatcher) =>
{
    var result = await queryDispatcher.QueryAsync(new GetProducts());
    return Results.Ok(result);
})
.WithName("GetProducts");

app.MapPost("/order", async (ICommandDispatcher commandDispatcher, PlaceOrder command) =>
{
    await commandDispatcher.DispatchAsync(command);
    return Results.Created("/order", null);
})
.WithName("PlaceOrder");

app.Run();
