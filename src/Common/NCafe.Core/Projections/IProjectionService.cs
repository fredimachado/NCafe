using NCafe.Core.Domain;
using NCafe.Core.ReadModels;

namespace NCafe.Core.Projections;

public delegate Guid GetModelId<in TEvent>(TEvent @event) where TEvent : class, IEvent;
public delegate void ModelUpdate<in TEvent, T>(TEvent @event, T model) where TEvent : class, IEvent;

public interface IProjectionService<T> where T : ReadModel
{
    Task Start(CancellationToken cancellationToken);
    void OnCreate<TEvent>(Func<TEvent, T> handler) where TEvent : class, IEvent;
    void OnUpdate<TEvent>(GetModelId<TEvent> getId, ModelUpdate<TEvent, T> update) where TEvent : class, IEvent;
}
