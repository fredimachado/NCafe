using NCafe.Core.ReadModels;
using System.Collections.Concurrent;

namespace NCafe.Infrastructure.ReadModels;

internal class InMemoryReadModelRepository<T> : IReadModelRepository<T> where T : ReadModel
{
    private readonly static ConcurrentDictionary<Guid, T> _items = new();

    public void Add(T model)
    {
        _items[model.Id] = model;
    }

    public IEnumerable<T> GetAll()
    {
        return [.. _items.Values];
    }

    public T GetById(Guid id)
    {
        return _items.TryGetValue(id, out var result) ? result : null;
    }
}
