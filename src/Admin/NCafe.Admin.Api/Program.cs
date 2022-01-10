using NCafe.Abstractions.Commands;
using NCafe.Admin.Application.Commands;
using NCafe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCommandHandlers(typeof(CreateProduct).Assembly);

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

app.MapPost("/products", async (ICommandDispatcher commandDispatcher, CreateProduct command) =>
{
    await commandDispatcher.DispatchAsync(command);
    return Results.Created("/products", null);
})
.WithName("CreateProduct");

app.Run();
