using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCafe.Abstractions.Commands;
using NCafe.Abstractions.EventBus;
using NCafe.Abstractions.Queries;
using NCafe.Abstractions.ReadModels;
using NCafe.Abstractions.Repositories;
using NCafe.Infrastructure.Commands;
using NCafe.Infrastructure.EventStore;
using NCafe.Infrastructure.Logging;
using NCafe.Infrastructure.MessageBus;
using NCafe.Infrastructure.Queries;
using NCafe.Infrastructure.ReadModels;
using System.Reflection;

namespace NCafe.Infrastructure;

public static class DependencyRegistration
{
    public static IServiceCollection AddEventStoreRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(e =>
        {
            var settings = EventStoreClientSettings.Create(configuration.GetConnectionString("eventstore:http"));
            var client = new EventStoreClient(settings);
            return client;
        });

        services.AddTransient<IRepository, EventStoreRepository>();

        return services;
    }

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton<IQueryDispatcher, QueryDispatcher>();

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddCommandHandlerLogger(this IServiceCollection services)
    {
        services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerLogger<>));

        return services;
    }

    public static IServiceCollection AddKafkaPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection(KafkaOptions.SectionKey);
        var options = new KafkaOptions();
        section.Bind(options);

        if (string.IsNullOrWhiteSpace(options.BootstrapServers))
        {
            throw new InvalidOperationException("Invalid Kafka configuration.");
        }

        services.Configure<KafkaOptions>(section);
        services.AddSingleton<IPublisher, KafkaPublisher>();

        return services;
    }

    public static IServiceCollection AddInMemoryReadModelRepository<T>(this IServiceCollection services) where T : ReadModel
    {
        services.AddSingleton<IReadModelRepository<T>, InMemoryReadModelRepository<T>>();

        return services;
    }

}
