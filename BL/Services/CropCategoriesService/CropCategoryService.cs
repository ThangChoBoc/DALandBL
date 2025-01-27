using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using ZelnyTrh.EF.BL.DTOs.CropCategoriesDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Enums;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.CropCategoriesService;

public class CropCategoryService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    UserManager<ApplicationUser> userManager) : ICropCategoryService
{

    public async Task<CropCategoryReadDto> CreateCategoryAsync(CropCategoryCreateDto dto)
    {
        var categoryRepository = unitOfWork.GetRepository<CropCategories>();

        // Validate parent category if specified
        if (dto.ParentCategoryId != null)
        {
            var parentExists = await categoryRepository.ExistsAsync(dto.ParentCategoryId);
            if (!parentExists)
                throw new InvalidOperationException("Parent category does not exist");
        }

        // Validate user exists
        var user = await userManager.FindByIdAsync(dto.ProposedByUserId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Create new category
        var category = new CropCategories
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            ParentCategoryId = dto.ParentCategoryId,
            CropCategoryStatus = dto.Status
        };

        // Add attribute definitions
        foreach (var attrDto in dto.AttributeDefinitions)
        {
            var attribute = new AttributeDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Name = attrDto.Name,
                DataType = attrDto.DataType,
                IsRequired = attrDto.IsRequired,
                ValidationRule = attrDto.ValidationRule,
                Unit = attrDto.Unit,
                CategoryId = category.Id,
                Category = category
            };

            category.AttributeDefinitions.Add(attribute);
        }

        await categoryRepository.InsertAsync(category);
        await unitOfWork.CommitAsync();

        return mapper.Map<CropCategoryReadDto>(category);
    }

    public async Task<CropCategoryReadDto> GetCategoryByIdAsync(string id)
    {
        var category = await unitOfWork.GetRepository<CropCategories>().GetByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        var dto = mapper.Map<CropCategoryReadDto>(category);

        // Load children
        var children = await unitOfWork.GetRepository<CropCategories>()
            .GetByConditionAsync(c => c.ParentCategoryId == id);
        dto.ChildCategories = mapper.Map<List<CropCategoryReadDto>>(children);

        return dto;
    }

    public async Task<IEnumerable<CropCategoryReadDto>> GetRootCategoriesAsync()
    {
        var categoryRepository = unitOfWork.GetRepository<CropCategories>();

        var allCategories = await categoryRepository.GetAllAsync();

        var categoryDtos = mapper.Map<List<CropCategoryReadDto>>(allCategories);

        var categoryLookup = categoryDtos.ToDictionary(c => c.Id);

        foreach (var categoryDto in categoryDtos)
        {
            if (!string.IsNullOrEmpty(categoryDto.ParentCategoryId) && categoryLookup.ContainsKey(categoryDto.ParentCategoryId))
            {
                var parentCategory = categoryLookup[categoryDto.ParentCategoryId];
                parentCategory.ChildCategories.Add(categoryDto);
            }
        }

        var rootCategories = categoryDtos.Where(c => string.IsNullOrEmpty(c.ParentCategoryId)).ToList();

        return rootCategories;
    }

    public async Task<CropCategoryReadDto> UpdateCategoryStatusAsync(string id, CropCategoryStatus newStatus, string moderatorId)
    {
        // Verify moderator
        var moderator = await userManager.FindByIdAsync(moderatorId);
        if (moderator == null || (!await userManager.IsInRoleAsync(moderator, "Moderator") && !await userManager.IsInRoleAsync(moderator, "Administrator")))
            throw new UnauthorizedAccessException("Only moderators can update category status");

        var category = await unitOfWork.GetRepository<CropCategories>().GetByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        category.CropCategoryStatus = newStatus;
        await unitOfWork.GetRepository<CropCategories>().UpdateAsync(category);
        await unitOfWork.CommitAsync();

        return mapper.Map<CropCategoryReadDto>(category);
    }

    public async Task DeleteCategoryAsync(string id)
    {
        var categoryRepository = unitOfWork.GetRepository<CropCategories>();

        // Check if category has children
        var hasChildren = await categoryRepository.GetByConditionAsync(c => c.ParentCategoryId == id);
        if (hasChildren.Any())
            throw new InvalidOperationException("Cannot delete category with child categories");

        // Check if category has associated crops
        var hasOffers = await unitOfWork.GetRepository<Offers>()
            .GetByConditionAsync(o => o.Crop.CategoryId == id);
        if (hasOffers.Any())
            throw new InvalidOperationException("Cannot delete category with associated offers");

        await categoryRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<CropCategoryReadDto>> GetCategoryPathAsync(string categoryId)
    {
        var path = new List<CropCategories>();
        var visitedCategories = new HashSet<string>();
        var currentCategory = await unitOfWork.GetRepository<CropCategories>().GetByIdAsync(categoryId);

        while (currentCategory != null && !visitedCategories.Contains(currentCategory.Id))
        {
            path.Add(currentCategory);
            visitedCategories.Add(currentCategory.Id);
            if (string.IsNullOrEmpty(currentCategory.ParentCategoryId))
                break;

            try
            {
                currentCategory = await unitOfWork.GetRepository<CropCategories>().GetByIdAsync(currentCategory.ParentCategoryId);
            }
            catch (InvalidOperationException)
            {
                // Parent category not found, terminate the loop
                currentCategory = null;
            }
        }

        path.Reverse();
        return mapper.Map<IEnumerable<CropCategoryReadDto>>(path);
    }

    public async Task<bool> ValidateCategoryHierarchyAsync(string parentId, string? childId = null)
    {
        var parent = await unitOfWork.GetRepository<CropCategories>().GetByIdAsync(parentId);

        if (childId == null) return true;
        if (parentId == childId) return false;

        // Prevent circular references
        var current = parent;
        while (true)
        {
            if (current.Id == childId)
                return false;
            if (current.ParentCategoryId == null)
                break;
            current = await unitOfWork.GetRepository<CropCategories>().GetByIdAsync(current.ParentCategoryId);
        }

        return true;
    }

    public async Task<CropCategoryReadDto> UpdateCategoryAsync(CropCategoryUpdateDto dto)
    {
        var categoryRepo = unitOfWork.GetRepository<CropCategories>();
        var attributeRepo = unitOfWork.GetRepository<AttributeDefinition>();

        // Get existing category
        var category = await categoryRepo.GetByIdAsync(dto.Id);

        // Validate parent category if changed
        if (dto.ParentCategoryId != category.ParentCategoryId)
        {
            if (!string.IsNullOrEmpty(dto.ParentCategoryId))
            {
                var parentExists = await categoryRepo.ExistsAsync(dto.ParentCategoryId);
                if (!parentExists)
                    throw new KeyNotFoundException("Parent category not found");

                // Check for circular reference
                if (!await ValidateCategoryHierarchyAsync(dto.ParentCategoryId, dto.Id))
                    throw new InvalidOperationException("Cannot create circular reference in category hierarchy");
            }
        }

        try
        {
            // Update basic category info
            category.Name = dto.Name;
            category.ParentCategoryId = dto.ParentCategoryId;

            if (category.ParentCategoryId == "")
                category.ParentCategoryId = null;

            // Get existing attributes
            var existingAttributes = await attributeRepo.GetByConditionAsync(a => a.CategoryId == category.Id);
            var existingAttributeDict = existingAttributes.ToDictionary(a => a.Id);

            // Process attributes
            foreach (var attrDto in dto.AttributeDefinitions)
            {
                if (string.IsNullOrEmpty(attrDto.Id) || !existingAttributeDict.ContainsKey(attrDto.Id))
                {
                    // New attribute
                    var newAttr = new AttributeDefinition
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = attrDto.Name,
                        DataType = attrDto.DataType,
                        IsRequired = attrDto.IsRequired,
                        CategoryId = category.Id,
                        Category = category
                    };
                    await attributeRepo.InsertAsync(newAttr);
                }
                else
                {
                    // Update existing attribute
                    var existingAttr = existingAttributeDict[attrDto.Id];
                    existingAttr.Name = attrDto.Name;
                    existingAttr.DataType = attrDto.DataType;
                    existingAttr.IsRequired = attrDto.IsRequired;
                    await attributeRepo.UpdateAsync(existingAttr);
                    existingAttributeDict.Remove(attrDto.Id);
                }
            }

            // Delete attributes that were not included in the update
            foreach (var removedAttr in existingAttributeDict.Values)
            {
                await attributeRepo.DeleteAsync(removedAttr.Id);
            }

            // Save all changes
            await unitOfWork.CommitAsync();

            // Return updated category
            return await GetCategoryByIdAsync(category.Id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to update category: {ex.Message}", ex);
        }
    }

    public async Task<int> DeleteRejectedCategoriesAsync(string moderatorId)
    {
        try
        {
            // Verify moderator
            var moderator = await userManager.FindByIdAsync(moderatorId);
            if (moderator == null || (!await userManager.IsInRoleAsync(moderator, "Moderator") &&
                                      !await userManager.IsInRoleAsync(moderator, "Administrator")))
            {
                throw new UnauthorizedAccessException("Only moderators can perform this action");
            }

            var categoryRepository = unitOfWork.GetRepository<CropCategories>();

            // Get all rejected categories
            var rejectedCategories = await categoryRepository.GetByConditionAsync(
                c => c.CropCategoryStatus == CropCategoryStatus.Rejected);

            var count = 0;
            foreach (var category in rejectedCategories)
            {
                // Check if category can be safely deleted (no child categories or associated crops)
                var hasChildren = await categoryRepository.GetByConditionAsync(
                    c => c.ParentCategoryId == category.Id);

                var hasAssociatedCrops = await unitOfWork.GetRepository<Crops>()
                    .GetByConditionAsync(c => c.CategoryId == category.Id);

                if (!hasChildren.Any() && !hasAssociatedCrops.Any())
                {
                    await categoryRepository.DeleteAsync(category.Id);
                    count++;
                }
            }

            await unitOfWork.CommitAsync();
            return count;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete rejected categories: {ex.Message}", ex);
        }
    }
}
