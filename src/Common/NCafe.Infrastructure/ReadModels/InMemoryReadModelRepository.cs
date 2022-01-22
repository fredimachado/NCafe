using NCafe.Core.ReadModels;
using System.Collections.Concurrent;

namespace NCafe.Infrastructure.ReadModels;

internal class InMemoryReadModelRepository<T> : IReadModelRepository<T> where T : ReadModel
{
    private static readonly ConcurrentDictionary<Guid, T> items = new();

    public void Add(T model)
    {
        items[model.Id] = model;
    }

    public IEnumerable<T> GetAll()
    {
        return items.Values.ToList();
    }

    public T GetById(Guid id)
    {
        return items.TryGetValue(id, out var result) ? result : null;
    }
}
