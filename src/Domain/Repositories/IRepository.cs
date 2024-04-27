namespace Infra.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll(int? page = null, int? pageQuantity = null);
    T? GetById(Guid id);
    bool Add(T entity);
    Task<int> AddAsync(T entity);
    bool AddRange(IEnumerable<T> entities);
    bool Update(T entity);
    bool UpdateRange(IEnumerable<T> entities);
    void Remove(Guid id);
}