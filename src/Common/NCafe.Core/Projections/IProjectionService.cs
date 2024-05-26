using NCafe.Core.Domain;
using NCafe.Core.ReadModels;

namespace NCafe.Core.Projections;

public delegate Guid GetModelId<in TEvent>(TEvent @event) where TEvent : Event;
public delegate void ModelUpdate<in TEvent, T>(TEvent @event, T model) where TEvent : Event;

public interface IProjectionService<T> where T : ReadModel
{
    Task Start(CancellationToken cancellationToken);
    void OnCreate<TEvent>(Func<TEvent, T> handler) where TEvent : Event;
    void OnUpdate<TEvent>(GetModelId<TEvent> getId, ModelUpdate<TEvent, T> update) where TEvent : Event;
}
