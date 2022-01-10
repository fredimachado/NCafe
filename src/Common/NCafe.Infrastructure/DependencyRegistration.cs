using Microsoft.Extensions.DependencyInjection;
using NCafe.Abstractions.Commands;
using NCafe.Infrastructure.Commands;
using System.Reflection;

namespace NCafe.Infrastructure;

public static class DependencyRegistration
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
