using NCafe.Core.Commands;
using NCafe.Core.Queries;
using NCafe.Admin.Api.Projections;
using NCafe.Admin.Domain.Commands;
using NCafe.Admin.Domain.Queries;
using NCafe.Admin.Domain.ReadModels;
using NCafe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddCommandHandlers(typeof(CreateProduct).Assembly)
                .AddCommandHandlerLogger()
                .AddQueryHandlers(typeof(CreateProduct).Assembly);

builder.Services.AddInMemoryReadModelRepository<Product>()
                .AddEventStoreProjectionService<Product>()
                .AddHostedService<ProductProjectionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseCors(corsPolicyName);

app.MapGet("/products", async (IQueryDispatcher queryDispatcher) =>
{
    var result = await queryDispatcher.QueryAsync(new GetProducts());
    return Results.Ok(result);
})
.WithName("GetProducts");

app.MapPost("/products", async (ICommandDispatcher commandDispatcher, CreateProduct command) =>
{
    await commandDispatcher.DispatchAsync(command);
    return Results.Created("/products", null);
})
.WithName("CreateProduct");

app.Run();
