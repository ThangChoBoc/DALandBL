using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL.Mappers;

public class OffersEntityMapper : IEntityMapper<Offers>
{
    public void MapToExistingEntity(Offers existingEntity, Offers newEntity)
    {
        if (existingEntity == null) throw new ArgumentNullException(nameof(existingEntity));
        if (newEntity == null) throw new ArgumentNullException(nameof(newEntity));

        // Map properties from newEntity to existingEntity
        existingEntity.Name = newEntity.Name;
        existingEntity.Price = newEntity.Price;
        existingEntity.Currency = newEntity.Currency;
        existingEntity.Amount = newEntity.Amount;
        existingEntity.UnitsAvailable = newEntity.UnitsAvailable;
        existingEntity.Origin = newEntity.Origin;
        existingEntity.OfferType = newEntity.OfferType;
    }
}