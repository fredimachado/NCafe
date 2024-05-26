using MediatR;
using Microsoft.Extensions.Logging;

namespace NCafe.Infrastructure.Behaviors;

internal class RequestHandlerLoggingBehavior<TRequest, TResponse>(ILogger<RequestHandlerLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly ILogger<RequestHandlerLoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;

        _logger.LogInformation("Started processing {RequestName} request.", requestName);
        var response = await next();
        _logger.LogInformation("Finished processing {RequestName} command.", requestName);

        return response;
    }
}
