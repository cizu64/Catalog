using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Catalog.API.Infrastructure.Repository;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    internal DbContext _context;
    internal DbSet<TEntity> _set;
    public GenericRepository(DbContext context)
    {
        _context = context;
        _set = _context.Set<TEntity>();
    }

    /// <summary>
    /// Returns a collection of ordered items
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
    {

        if (predicate != null)
        {
            return _set.Where(predicate);
        }
        return _set;
    }
    /// <summary>
    /// Get a single item asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<TEntity> GetAsync(int id)
    {
        return await _set.FindAsync(id);
    }
    /// <summary>
    /// Get a single item synchronously
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TEntity Get(int id)
    {
        return _set.Find(id);
    }
    /// <summary>
    /// Get a single item based on the expression in an asynchronous manner
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _set.FirstOrDefaultAsync(predicate);
    }
    /// <summary>
    /// Get a single item based on the expression in an synchronous manner
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public TEntity Get(Expression<Func<TEntity, bool>> predicate)
    {
        return _set.FirstOrDefault(predicate);
    }
    /// <summary>
    /// Counts a particular item in an asynchronous manner
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _set.CountAsync(predicate);
    }

    /// <summary>
    /// Counts a particular item in an synchronous manner
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int Count(Expression<Func<TEntity, bool>> predicate)
    {
        return _set.Count(predicate);
    }

    public async Task<long> LongCountAsync()
    {
        return await _set.LongCountAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _set.AnyAsync(predicate);
    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return _set.Any(predicate);
    }

    /// <summary>
    /// Add item
    /// </summary>
    /// <param name="entity"></param>
    public async Task Add(TEntity entity)
    {
        await _set.AddAsync(entity);
    }

    /// <summary>
    /// Add range
    /// </summary>
    /// <param name="entity"></param>
    public async Task AddRange(TEntity entity)
    {
        await _set.AddRangeAsync(entity);
    }

    public void Attach(TEntity entity)
    {
        _set.Attach(entity);
    }
    /// <summary>
    /// Delete a particular item
    /// </summary>
    /// <param name="entity"></param>
    public async Task Delete(TEntity entity)
    {
        _set.Remove(entity);
    }

    /// <summary>
    /// Delete range of entities
    /// </summary>
    /// <param name="entities"></param>
    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _set.RemoveRange(entities);
    }
    public EntityEntry Entry(object entity)
    {
        return _context.Entry(entity);
    }
}
