using EventStore.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCafe.Core.MessageBus;
using NCafe.Core.Projections;
using NCafe.Core.ReadModels;
using NCafe.Core.Repositories;
using NCafe.Infrastructure.Behaviors;
using NCafe.Infrastructure.EventStore;
using NCafe.Infrastructure.MessageBrokers.RabbitMQ;
using NCafe.Infrastructure.ReadModels;
using RabbitMQ.Client;

namespace NCafe.Infrastructure;

public static class DependencyRegistration
{
    public static IServiceCollection AddMediatR<T>(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<T>();
            config.AddOpenBehavior(typeof(RequestHandlerLoggingBehavior<,>));
        });

        return services;
    }

    public static IServiceCollection AddEventStoreRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(e =>
        {
            var settings = EventStoreClientSettings.Create(configuration.GetConnectionString("EventStore"));
            var client = new EventStoreClient(settings);
            return client;
        });

        services.AddTransient<IRepository, EventStoreRepository>();

        return services;
    }

    public static IServiceCollection AddEventStoreProjectionService<TModel>(this IServiceCollection services, IConfiguration configuration)
        where TModel : ReadModel
    {
        if (configuration.GetValue("EventStore:EnableProjections", false))
        {
            services.AddSingleton<IProjectionService<TModel>, EventStoreProjectionService<TModel>>();
        }

        return services;
    }

    public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("RabbitMq")))
        {
            // Fail fast. Service shouldn't be able to start with invalid configuration.
            throw new InvalidOperationException("Invalid RabbitMq configuration.");
        }

        services.AddOptions<RabbitMqSettings>()
                .Bind(configuration.GetSection(RabbitMqSettings.SectionName));

        services.AddSingleton<IBusPublisher, RabbitMqPublisher>();

        if (configuration.GetValue("RabbitMq:InitializeExchange", false))
        {
            InitializeRabbitMqExchange(services);
        }

        return services;
    }

    public static IServiceCollection AddRabbitMqConsumerService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqSettings>()
                .Bind(configuration.GetSection(RabbitMqSettings.SectionName));

        return services.AddSingleton<IMessageSubscriber, RabbitMqMessageSubscriber>()
                       .AddHostedService<RabbitMqConsumerService>();
    }

    public static IMessageSubscriber UseMessageSubscriber(this IApplicationBuilder app)
    {
        var messageSubscriber = app.ApplicationServices.GetService<IMessageSubscriber>();

        return messageSubscriber ??
            throw new InvalidOperationException("Message Subscriber is not registered. Make sure to call Services.AddRabbitMqConsumerService.");
    }

    private static void InitializeRabbitMqExchange(IServiceCollection services)
    {
        var scope = services.BuildServiceProvider().CreateScope();
        var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
        var settings = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqSettings>>();

        var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: ExchangeNameProvider.Get(settings.Value.ExchangeName),
                                type: ExchangeType.Topic,
                                durable: true,
                                autoDelete: false);
    }

    public static IServiceCollection AddInMemoryReadModelRepository<T>(this IServiceCollection services) where T : ReadModel
    {
        services.AddSingleton<IReadModelRepository<T>, InMemoryReadModelRepository<T>>();

        return services;
    }

}
