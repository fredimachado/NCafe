using MediatR;
using NCafe.Cashier.Api.Projections;
using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.Queries;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRabbitMQClient("rabbitmq");

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddEventStoreProjectionService<Product>(builder.Configuration)
                .AddInMemoryReadModelRepository<Product>()
                .AddHostedService<ProductProjectionService>();

builder.Services.AddRabbitMqPublisher(builder.Configuration);

builder.Services.AddMediatR<CreateOrder>();

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

app.MapGet("/products", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetProducts());
    return Results.Ok(result);
})
.WithName("GetProducts");

app.MapPost("/orders", async (IMediator mediator, CreateOrder command) =>
{
    var orderId = await mediator.Send(command);
    return Results.Created("/orders", orderId);
})
.WithName("CreateOrder");

app.MapPost("/orders/add-item", async (IMediator mediator, AddItemToOrder command) =>
{
    await mediator.Send(command);
    return Results.Accepted();
})
.WithName("AddItemToOrder");

app.MapPost("/orders/remove-item", async (IMediator mediator, RemoveItemFromOrder command) =>
    {
        await mediator.Send(command);
        return Results.Accepted();
    })
    .WithName("RemoveItemFromOrder");

app.MapPost("/orders/place", async (IMediator mediator, PlaceOrder command) =>
{
    await mediator.Send(command);
    return Results.Accepted();
})
.WithName("PlaceOrder");

app.MapGet("/healthz", () => "OK");

app.Run();

public partial class Program;
