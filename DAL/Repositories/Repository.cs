using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZelnyTrh.EF.DAL.Entities;
namespace ZelnyTrh.EF.DAL.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity<string>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<TEntity>();
    }

    public Task<IQueryable<TEntity>> GetAllAsync()
    {
        return Task.FromResult(_dbSet.AsQueryable());
    }

    public async Task<TEntity> GetByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        return await _dbSet.FindAsync(id) ?? throw new InvalidOperationException();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        return await _dbSet.AnyAsync(e => e.Id == id);
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(nameof(entity));
        await _dbSet.AddAsync(entity);
        return entity;
    }
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync(); // Persist changes asynchronously
        return entity;
    }

    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
            throw new InvalidOperationException($"Entity with ID {id} not found.");
        _dbSet.Remove(entity);
    }

    public async Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
    }

}