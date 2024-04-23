using Domain.Base;
using Infra.Context;

namespace Infra.Repositories;

public abstract class Repository<T> where T : BaseEntity
{
    public readonly DmContext _context;

    public Repository(DmContext context)
    {
        _context = context;
    }

    public IEnumerable<T> GetAll()
    {
        try
        {
            return _context.Set<T>().Where(x => !x.Excluded);
        }
        catch (Exception)
        {
            return Enumerable.Empty<T>();
        }
    } 
    public T? GetById(Guid id) => _context.Set<T>().FirstOrDefault(x => x.Id == id);

    public bool Add(T entity)
    {
        try
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public Task<int> AddAsync(T entity) {
        _context.Set<T>().AddAsync(entity);
        return _context.SaveChangesAsync();
    }
    
    public bool AddRange(IEnumerable<T> entities)
    {
        try
        {
            _context.Set<T>().AddRange(entities);
            _context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public bool Update(T entity)
    {
        try
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public bool UpdateRange(IEnumerable<T> entities)
    {
        try
        {
            _context.Set<T>().UpdateRange(entities);
            _context.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public void Remove(Guid id)
    {
        var entity =_context.Set<T>().FirstOrDefault(x => x.Id == id);
        if (entity != null)
        {
            entity.Excluded = true;
        }
        _context.SaveChanges();
    } 
}