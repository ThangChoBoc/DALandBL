using System.Linq.Expressions;
using ZelnyTrh.EF.DAL.Entities;
namespace ZelnyTrh.EF.DAL.Repositories;

public interface IRepository<TEntity>
    where TEntity : class, IEntity<string>
{
    Task<IQueryable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> InsertAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(string id);
}