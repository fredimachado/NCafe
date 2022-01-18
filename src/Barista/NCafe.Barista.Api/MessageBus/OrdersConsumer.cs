using NCafe.Abstractions.Commands;
using NCafe.Abstractions.MessageBus.Events;
using NCafe.Barista.Domain.Commands;

namespace NCafe.Barista.Api.MessageBus;

public class OrdersConsumer : IHostedService
{
    private const string Topic = "orders";

    private readonly ICommandDispatcher commandDispatcher;
    private readonly ILogger<OrdersConsumer> logger;

    public OrdersConsumer(
        ICommandDispatcher commandDispatcher,
        IConfiguration configuration,
        ILogger<OrdersConsumer> logger)
    {
        this.commandDispatcher = commandDispatcher;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
