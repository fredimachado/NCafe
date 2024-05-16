using Microsoft.Extensions.DependencyInjection;
using NCafe.Core.Commands;

namespace NCafe.Infrastructure.Commands;

internal sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        await handler.HandleAsync(command);
    }
}
