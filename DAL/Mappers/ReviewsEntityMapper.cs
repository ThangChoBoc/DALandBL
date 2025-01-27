using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL.Mappers;

public class ReviewsEntityMapper : IEntityMapper<Reviews>
{
    public void MapToExistingEntity(Reviews existingEntity, Reviews newEntity)
    {
        if (existingEntity == null) throw new ArgumentNullException(nameof(existingEntity));
        if (newEntity == null) throw new ArgumentNullException(nameof(newEntity));

        existingEntity.Rating = newEntity.Rating;
        existingEntity.ReviewDescription = newEntity.ReviewDescription;
    }
}