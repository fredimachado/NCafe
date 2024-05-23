using NCafe.Cashier.Api.Projections;
using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Queries;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.Commands;
using NCafe.Core.Queries;
using NCafe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRabbitMQClient("rabbitmq");

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddCommandHandlers<PlaceOrder>()
                .AddCommandHandlerLogger()
                .AddQueryHandlers<PlaceOrder>();

builder.Services.AddInMemoryReadModelRepository<Product>()
                .AddEventStoreProjectionService<Product>()
                .AddHostedService<ProductProjectionService>();

builder.Services.AddRabbitMqPublisher(builder.Configuration);

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

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyName);

app.MapGet("/products", async (IQueryDispatcher queryDispatcher) =>
{
    var result = await queryDispatcher.QueryAsync(new GetProducts());
    return Results.Ok(result);
})
.WithName("GetProducts");

app.MapPost("/orders", async (ICommandDispatcher commandDispatcher, CreateOrder command) =>
{
    await commandDispatcher.DispatchAsync(command);
    return Results.Created("/orders", null);
})
.WithName("CreateOrder");

app.MapPost("/orders/{id:guid}/item", async (ICommandDispatcher commandDispatcher, Guid id, AddItemToOrder command) =>
{
    await commandDispatcher.DispatchAsync(command);
    return Results.Accepted();
})
.WithName("AddItemToOrder");

app.MapPost("/orders/{id:guid}/place", async (ICommandDispatcher commandDispatcher, Guid id, PlaceOrder command) =>
{
    await commandDispatcher.DispatchAsync(command);
    return Results.Accepted();
})
.WithName("PlaceOrder");

app.MapGet("/healthz", () => "OK");

app.Run();
