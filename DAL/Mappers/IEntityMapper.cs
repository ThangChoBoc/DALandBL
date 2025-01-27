using ZelnyTrh.EF.DAL.Entities;
namespace ZelnyTrh.EF.DAL.Mappers;
public interface IEntityMapper<in TEntity>
    where TEntity : IEntity<string>
{
    void MapToExistingEntity(TEntity existingEntity, TEntity newEntity);
}