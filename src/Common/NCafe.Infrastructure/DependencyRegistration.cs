using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCafe.Core.Commands;
using NCafe.Core.MessageBus;
using NCafe.Core.Queries;
using NCafe.Core.ReadModels;
using NCafe.Core.Repositories;
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
            var settings = EventStoreClientSettings.Create(configuration.GetConnectionString("EventStore"));
            var client = new EventStoreClient(settings);
            return client;
        });

        services.AddTransient<IRepository, EventStoreRepository>();

        return services;
    }

    public static IServiceCollection AddEventStoreProjectionService<TModel>(this IServiceCollection services)
        where TModel : ReadModel
    {
        services.AddSingleton<EventStoreProjectionService<TModel>>();

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

    public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("RabbitMq")))
        {
            // Fail fast. Service shouldn't be able to start with invalid configuration.
            throw new InvalidOperationException("Invalid RabbitMq configuration.");
        }

        services.AddSingleton<IPublisher, RabbitMqPublisher>();

        return services;
    }

    public static IServiceCollection AddInMemoryReadModelRepository<T>(this IServiceCollection services) where T : ReadModel
    {
        services.AddSingleton<IReadModelRepository<T>, InMemoryReadModelRepository<T>>();

        return services;
    }

}
