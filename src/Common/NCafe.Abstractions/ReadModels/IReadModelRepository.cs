namespace NCafe.Abstractions.ReadModels;

public interface IReadModelRepository<T> where T : ReadModel
{
    void Add(T model);
    T GetById(Guid id);
    IEnumerable<T> GetAll();
}
