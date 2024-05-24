using MediatR;
using NCafe.Admin.Api.Projections;
using NCafe.Admin.Domain.Commands;
using NCafe.Admin.Domain.Queries;
using NCafe.Admin.Domain.ReadModels;
using NCafe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddEventStoreRepository(builder.Configuration)
                .AddEventStoreProjectionService<Product>()
                .AddInMemoryReadModelRepository<Product>()
                .AddHostedService<ProductProjectionService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateProduct>());

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

app.MapPost("/products", async (IMediator mediator, CreateProduct command) =>
{
    await mediator.Send(command);
    return Results.Created("/products", null);
})
.WithName("CreateProduct");

app.MapGet("/healthz", () => "OK");

app.Run();
