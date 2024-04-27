using Domain.Base;
using Infra.Context;
using Microsoft.Extensions.Logging;

namespace Infra.Repositories;

public abstract class Repository<T> where T : BaseEntity
{
    protected readonly DmContext _context;
    private readonly ILogger<Repository<T>> _logger;

    public Repository(
        DmContext context,
        ILogger<Repository<T>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IEnumerable<T> GetAll(int? page = null, int? pageQuantity = null)
    {
        try
        {
            if (page.HasValue && pageQuantity.HasValue)
                return _context.Set<T>().Skip(page.Value * pageQuantity.Value).Take(pageQuantity.Value).Where(x => !x.Excluded);
            
            return _context.Set<T>().Where(x => !x.Excluded);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Critical, ex.Message);
            return Enumerable.Empty<T>();
        }
    } 
    public T? GetById(Guid id) => _context.Set<T>().FirstOrDefault(x => !x.Excluded && x.Id == id);

    public bool Add(T entity)
    {
        try
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Critical, ex.Message);
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
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Critical, ex.Message);
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
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Critical, ex.Message);
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
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Critical, ex.Message);
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