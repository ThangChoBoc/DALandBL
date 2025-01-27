using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL.Mappers;

public class CropsEntityMapper : IEntityMapper<Crops>
{
    public void MapToExistingEntity(Crops existingEntity, Crops newEntity)
    {
        if (existingEntity == null) throw new ArgumentNullException(nameof(existingEntity));
        if (newEntity == null) throw new ArgumentNullException(nameof(newEntity));

        // Map scalar properties from newEntity to existingEntity
        existingEntity.Name = newEntity.Name;
    }
}
