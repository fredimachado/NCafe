﻿using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NCafe.Cashier.Domain.ReadModels;
using NCafe.Core.MessageBus;
using NCafe.Core.ReadModels;
using NCafe.Core.Repositories;

namespace NCafe.Cashier.Api.Tests;

public class CashierWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly IRepository _repository;
    private readonly IReadModelRepository<Product> _productRepository;
    private readonly IBusPublisher _busPublisher;

    public IRepository FakeRepository => _repository;
    public IReadModelRepository<Product> FakeProductRepository => _productRepository;
    public IBusPublisher FakeBusPublisher => _busPublisher;

    public CashierWebApplicationFactory()
    {
        _repository = A.Fake<IRepository>();
        _productRepository = A.Fake<IReadModelRepository<Product>>();
        _busPublisher = A.Fake<IBusPublisher>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("RabbitMq__InitializeExchange", "false");

        builder.ConfigureServices(services =>
        {
            var repositoryDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IRepository));
            services.Remove(repositoryDescriptor!);

            var productRepositoryDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IReadModelRepository<Product>));
            services.Remove(productRepositoryDescriptor!);

            var busPublisherDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IBusPublisher));
            services.Remove(busPublisherDescriptor!);

            services.AddSingleton(_repository);
            services.AddSingleton(_productRepository);
            services.AddSingleton(_busPublisher);
        });

        builder.UseEnvironment("Tests");
    }
}
