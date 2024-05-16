using Microsoft.Extensions.Logging;
using NCafe.Core.Commands;

namespace NCafe.Infrastructure.Logging;

internal sealed class CommandHandlerLogger<TCommand>(ICommandHandler<TCommand> handler, ILogger<CommandHandlerLogger<TCommand>> logger)
    : ICommandHandler<TCommand> where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler = handler;
    private readonly ILogger _logger = logger;

    public async Task HandleAsync(TCommand command)
    {
        var commandType = command.GetType().Name;

        try
        {
            _logger.LogInformation("Started processing {commandType} command.", commandType);
            await _handler.HandleAsync(command);
            _logger.LogInformation("Finished processing {commandType} command.", commandType);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process {commandType} command.", commandType);
            throw;
        }
    }
}
