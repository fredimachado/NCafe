using NCafe.Abstractions.Commands;
using NCafe.Cashier.Application.Commands;
using NCafe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCommandHandlers(typeof(PlaceOrder).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/order", async (ICommandDispatcher commandDispatcher, PlaceOrder command) =>
{
    await commandDispatcher.DispatchAsync(command);
    return Results.Created("/order", null);
})
.WithName("PlaceOrder");

app.Run();
