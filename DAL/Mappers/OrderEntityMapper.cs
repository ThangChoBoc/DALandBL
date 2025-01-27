using ZelnyTrh.EF.DAL.Entities;
namespace ZelnyTrh.EF.DAL.Mappers;
public class OrderEntityMapper : IEntityMapper<Orders>
{
    public void MapToExistingEntity(Orders existingEntity, Orders newEntity)
    {
        if (existingEntity == null) throw new ArgumentNullException(nameof(existingEntity));
        if (newEntity == null) throw new ArgumentNullException(nameof(newEntity));

        existingEntity.Amount = newEntity.Amount;
        existingEntity.Price = newEntity.Price;
        existingEntity.PaymentType = newEntity.PaymentType;
        existingEntity.OrderStatus = newEntity.OrderStatus;
    }
}