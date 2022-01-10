using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NCafe.Abstractions.Commands;
using NCafe.Abstractions.Queries;
using NCafe.Abstractions.Repositories;
using NCafe.Infrastructure.Commands;
using NCafe.Infrastructure.EventStore;
using NCafe.Infrastructure.Queries;
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
}
