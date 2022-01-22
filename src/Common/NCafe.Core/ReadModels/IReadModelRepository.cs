namespace NCafe.Core.ReadModels;

public interface IReadModelRepository<T> where T : ReadModel
{
    void Add(T model);
    T GetById(Guid id);
    IEnumerable<T> GetAll();
}
