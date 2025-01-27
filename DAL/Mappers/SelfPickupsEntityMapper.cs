using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL.Mappers;

public class SelfPickupEntityMapper : IEntityMapper<SelfPickups>
{
    public void MapToExistingEntity(SelfPickups existingEntity, SelfPickups newEntity)
    {
        if (existingEntity == null)
            throw new ArgumentNullException(nameof(existingEntity));
        if (newEntity == null)
            throw new ArgumentNullException(nameof(newEntity));

        // Map properties from newEntity to existingEntity
        existingEntity.Location = newEntity.Location;
        existingEntity.Starting = newEntity.Starting;
        existingEntity.Ending = newEntity.Ending;
    }
}