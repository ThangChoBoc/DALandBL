using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL.Mappers;

public class ApplicationUserEntityMapper : IEntityMapper<ApplicationUser>
{
    public void MapToExistingEntity(ApplicationUser existingEntity, ApplicationUser newEntity)
    {
        ArgumentNullException.ThrowIfNull(nameof(existingEntity));
        ArgumentNullException.ThrowIfNull(nameof(newEntity));

        // Map properties from newEntity to existingEntity
        existingEntity.Name = newEntity.Name;
    }
}
