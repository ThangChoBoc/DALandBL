using System.Threading.Tasks;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Repositories;

namespace ZelnyTrh.EF.DAL.UnitsOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity<string>;
    Task<int> CommitAsync();
}
