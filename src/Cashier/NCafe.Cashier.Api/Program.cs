using NCafe.Abstractions.Commands;
using NCafe.Abstractions.Queries;
using NCafe.Cashier.Domain.Queries;
using NCafe.Cashier.Domain.Commands;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Infrastructure;
using NCafe.Cashier.Api.Projections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddCommandHandlers(typeof(PlaceOrder).Assembly)
                .AddCommandHandlerLogger()
                .AddQueryHandlers(typeof(PlaceOrder).Assembly);

builder.Services.AddInMemoryReadModelRepository<Product>()
                .AddEventStoreProjectionService<Product>()
                .AddHostedService<ProductProjectionService>();


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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);

app.MapGet("/products", async (IQueryDispatcher queryDispatcher) =>
{
    var result = await queryDispatcher.QueryAsync(new GetProducts());
    return Results.Ok(result);
})
.WithName("GetProducts");

app.MapPost("/orders", async (ICommandDispatcher commandDispatcher, PlaceOrder command) =>
{
    await commandDispatcher.DispatchAsync(command);
    return Results.Created("/orders", null);
})
.WithName("PlaceOrder");

app.Run();
