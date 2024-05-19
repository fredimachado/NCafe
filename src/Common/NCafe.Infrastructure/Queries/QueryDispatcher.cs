using Microsoft.Extensions.DependencyInjection;
using NCafe.Core.Queries;

namespace NCafe.Infrastructure.Queries;

internal sealed class QueryDispatcher(IServiceScopeFactory serviceScopeFactory) : IQueryDispatcher
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        return await (Task<TResult>)handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync))
            ?.Invoke(handler, [query]);
    }
}
