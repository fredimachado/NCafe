using Microsoft.Extensions.Logging;
using NCafe.Abstractions.Commands;

namespace NCafe.Infrastructure.Logging;

internal sealed class CommandHandlerLogger<TCommand> : ICommandHandler<TCommand> where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> handler;
    private readonly ILogger logger;

    public CommandHandlerLogger(ICommandHandler<TCommand> handler, ILogger<CommandHandlerLogger<TCommand>> logger)
    {
        this.handler = handler;
        this.logger = logger;
    }

    public async Task HandleAsync(TCommand command)
    {
        var commandType = command.GetType().Name;

        try
        {
            logger.LogInformation("Started processing {commandType} command.", commandType);
            await handler.HandleAsync(command);
            logger.LogInformation("Finished processing {commandType} command.", commandType);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process {commandType} command.", commandType);
            throw;
        }
    }
}
