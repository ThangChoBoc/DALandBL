using System;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL.Mappers;

public class CropCategoriesEntityMapper : IEntityMapper<CropCategories>
{
    public void MapToExistingEntity(CropCategories existingEntity, CropCategories newEntity)
    {
        if (existingEntity == null)
            throw new ArgumentNullException(nameof(existingEntity));
        if (newEntity == null)
            throw new ArgumentNullException(nameof(newEntity));

        // Map scalar properties
        existingEntity.Name = newEntity.Name;
        existingEntity.CropCategoryStatus = newEntity.CropCategoryStatus;

        // Update foreign keys
        existingEntity.ParentCategoryId = newEntity.ParentCategoryId;
    }
}
