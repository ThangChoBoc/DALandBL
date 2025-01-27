using System.Collections.Concurrent;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Repositories;

namespace ZelnyTrh.EF.DAL.UnitsOfWork;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity<string>
    {
        var type = typeof(TEntity);
        if (_repositories.TryGetValue(type, out var repository))
        {
            return (IRepository<TEntity>)repository;
        }

        var newRepository = new Repository<TEntity>(context);
        _repositories[type] = newRepository;
        return newRepository;
    }

    public async Task<int> CommitAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose managed resources
            context.Dispose();
        }
    }
}