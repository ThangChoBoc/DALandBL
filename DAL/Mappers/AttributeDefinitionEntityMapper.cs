using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL.Mappers;

public class AttributeDefinitionEntityMapper : IEntityMapper<AttributeDefinition>
{
    public void MapToExistingEntity(AttributeDefinition existingEntity, AttributeDefinition newEntity)
    {
        ArgumentNullException.ThrowIfNull(nameof(existingEntity));
        ArgumentNullException.ThrowIfNull(nameof(newEntity));

        // Map properties from newEntity to existingEntity
        existingEntity.Name = newEntity.Name;
        existingEntity.DataType = newEntity.DataType;
        existingEntity.IsRequired = newEntity.IsRequired;
        existingEntity.ValidationRule = newEntity.ValidationRule;
        existingEntity.Unit = newEntity.Unit;
        existingEntity.CategoryId = newEntity.CategoryId;
    }
}