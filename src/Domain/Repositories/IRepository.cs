namespace Infra.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(Guid id);
    bool Add(T entity);
    bool AddRange(IEnumerable<T> entities);
    bool Update(T entity);
    bool UpdateRange(IEnumerable<T> entities);
    void Remove(Guid id);
}