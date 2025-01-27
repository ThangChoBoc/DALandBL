using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZelnyTrh.EF.BL.DTOs.AttributesDto;
using ZelnyTrh.EF.BL.DTOs.CropDTO;
using ZelnyTrh.EF.BL.DTOs.OfferDTO;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.CropService;

public class CropService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<CropService> logger) : ICropService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CropService> _logger = logger;

    public async Task<CropReadDto> CreateCropAsync(CropCreateDto dto)
    {
        try
        {
            // Verify category exists and is approved
            var category = await _unitOfWork.GetRepository<CropCategories>()
                .GetByIdAsync(dto.CategoryId);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {dto.CategoryId} not found");

            if (category.CropCategoryStatus != ZelnyTrh.EF.DAL.Enums.CropCategoryStatus.Approved)
                throw new InvalidOperationException("Cannot create crops in unapproved categories");

            // Validate attributes against category definitions
            await ValidateCropAttributesAsync(dto.Attributes, category);

            // Create crop entity
            var crop = new Crops
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Category = category,
                CropAttributes = []
            };

            // Add attributes
            foreach (var attrDto in dto.Attributes)
            {
                var attrDef = category.AttributeDefinitions
                    .FirstOrDefault(ad => ad.Id == attrDto.AttributeDefinitionId);

                if (attrDef == null)
                    throw new ValidationException($"Attribute definition {attrDto.AttributeDefinitionId} not found");

                var attribute = new CropAttributes
                {
                    Id = Guid.NewGuid().ToString(),
                    CropId = crop.Id,
                    Crop = crop,
                    AttributeDefinitionId = attrDto.AttributeDefinitionId,
                    AttributeDefinition = attrDef,
                    Value = attrDto.Value
                };

                crop.CropAttributes.Add(attribute);
            }

            await _unitOfWork.GetRepository<Crops>().InsertAsync(crop);
            await _unitOfWork.CommitAsync();

            return await GetCropByIdAsync(crop.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating crop: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<CropListDto>> GetAllCropIdAsync()
    {
        try
        {
            var cropsQuery = await _unitOfWork.GetRepository<Crops>().GetAllAsync();

            var crops = await cropsQuery
                .Include(c => c.Category)
                .Include(c => c.Offers)
                .Include(c => c.CropAttributes)
                .ThenInclude(ca => ca.AttributeDefinition)
                .ThenInclude(ad => ad.Category)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CropListDto>>(crops);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all crops");
            throw;
        }
    }

    public async Task<CropReadDto> GetCropByIdAsync(string id)
    {
        var crop = await _unitOfWork.GetRepository<Crops>()
            .GetByConditionAsync(c => c.Id == id);

        var cropEntity = crop.FirstOrDefault();

        if (cropEntity == null)
            throw new KeyNotFoundException($"Crop with ID {id} not found");

        return _mapper.Map<CropReadDto>(cropEntity);
    }

    public async Task<CropSearchResultDto> SearchCropsAsync(CropSearchDto searchDto)
    {
        try
        {
            var query = await _unitOfWork.GetRepository<Crops>().GetAllAsync();

            // Apply category filter
            if (!string.IsNullOrEmpty(searchDto.CategoryId))
            {
                var categoryIds = await GetCategoryAndChildrenIds(searchDto.CategoryId);
                query = query.Where(c => categoryIds.Contains(c.CategoryId));
            }

            // Apply search term
            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                var searchTermLower = searchDto.SearchTerm.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchTermLower));
            }

            // Apply attribute filters
            foreach (var (attributeId, value) in searchDto.AttributeFilters)
            { 
                query = query.Where(c => c.CropAttributes
                    .Any(ca => ca.AttributeDefinitionId == attributeId && ca.Value == value));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, searchDto.SortBy, searchDto.SortDescending);

            // Apply pagination
            var items = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            // Get available attribute values for filtering
            var availableAttributeValues = await GetAvailableAttributeValues(query);

            return new CropSearchResultDto
            {
                Items = _mapper.Map<List<CropListDto>>(items),
                TotalCount = totalCount,
                PageCount = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize),
                AvailableAttributeValues = availableAttributeValues
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching crops: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<CropReadDto> UpdateCropAsync(CropUpdateDto dto)
    {
        try
        {
            var crop = await _unitOfWork.GetRepository<Crops>()
                .GetByIdAsync(dto.Id);

            if (crop == null)
                throw new KeyNotFoundException($"Crop with ID {dto.Id} not found");

            // Verify category if changed
            if (crop.CategoryId != dto.CategoryId)
            {
                var newCategory = await _unitOfWork.GetRepository<CropCategories>()
                    .GetByIdAsync(dto.CategoryId);

                if (newCategory == null)
                    throw new KeyNotFoundException($"Category with ID {dto.CategoryId} not found");

                if (newCategory.CropCategoryStatus != ZelnyTrh.EF.DAL.Enums.CropCategoryStatus.Approved)
                    throw new InvalidOperationException("Cannot move crop to unapproved category");

                crop.CategoryId = dto.CategoryId;
                crop.Category = newCategory;
            }

            // Update basic properties
            crop.Name = dto.Name;

            // Update attributes
            await UpdateCropAttributesAsync(crop, dto.Attributes);

            await _unitOfWork.GetRepository<Crops>().UpdateAsync(crop);
            await _unitOfWork.CommitAsync();

            return await GetCropByIdAsync(crop.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating crop: {Message}", ex.Message);
            throw;
        }
    }

    public async Task DeleteCropAsync(string id)
    {
        try
        {
            var crop = await _unitOfWork.GetRepository<Crops>()
                .GetByIdAsync(id);

            if (crop == null)
                throw new KeyNotFoundException($"Crop with ID {id} not found");

            if (crop.Offers.Any())
                throw new InvalidOperationException("Cannot delete crop with associated offers");

            await _unitOfWork.GetRepository<Crops>().DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting crop: {Message}", ex.Message);
            throw;
        }
    }

    private async Task ValidateCropAttributesAsync(
        ICollection<CropAttributeCreateDto> attributes,
        CropCategories category)
    {
        // Check required attributes
        var requiredAttributeIds = category.AttributeDefinitions
            .Where(ad => ad.IsRequired)
            .Select(ad => ad.Id)
            .ToHashSet();

        var providedAttributeIds = attributes
            .Select(a => a.AttributeDefinitionId)
            .ToHashSet();

        if (!requiredAttributeIds.All(id => providedAttributeIds.Contains(id)))
            throw new ValidationException("Not all required attributes are provided");

        // Validate each attribute
        foreach (var attr in attributes)
        {
            var definition = category.AttributeDefinitions
                .FirstOrDefault(ad => ad.Id == attr.AttributeDefinitionId);

            if (definition == null)
                throw new ValidationException($"Invalid attribute definition ID: {attr.AttributeDefinitionId}");

            if (!await ValidateAttributeValueAsync(attr.Value, definition))
                throw new ValidationException($"Invalid value for attribute {definition.Name}");
        }
    }
    private async Task<bool> ValidateAttributeValueAsync(string value, AttributeDefinition definition)
    {
        try
        {
            // Simulate I/O-bound work (e.g., database or external service calls)
            await Task.Delay(1);

            // Basic type validation
            switch (definition.DataType.ToLower())
            {
                case "number":
                    if (!decimal.TryParse(value, out _))
                        return false;
                    break;
                case "date":
                    if (!DateTime.TryParse(value, out _))
                        return false;
                    break;
                case "boolean":
                    if (!bool.TryParse(value, out _))
                        return false;
                    break;
            }

            // Validation rule checking if present
            if (!string.IsNullOrEmpty(definition.ValidationRule))
            {
                // Implement async validation logic if needed
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private Task<HashSet<string>> GetCategoryAndChildrenIds(string categoryId)
    {
        var result = new HashSet<string> { categoryId };
        var category = _unitOfWork.GetRepository<CropCategories>().GetByIdAsync(categoryId).Result;

        if (category == null)
            return Task.FromResult(result);

        CollectChildCategoryIds(category, result);
        return Task.FromResult(result);
    }

    private void CollectChildCategoryIds(CropCategories category, HashSet<string> ids)
    {
        foreach (var child in category.ChildCategories)
        {
            ids.Add(child.Id);
            CollectChildCategoryIds(child, ids);
        }
    }

    private static IQueryable<Crops> ApplySorting(
        IQueryable<Crops> query,
        string? sortBy,
        bool sortDescending)
    {
        query = sortBy?.ToLower() switch
        {
            "name" => sortDescending
                ? query.OrderByDescending(c => c.Name)
                : query.OrderBy(c => c.Name),
            "category" => sortDescending
                ? query.OrderByDescending(c => c.Category.Name)
                : query.OrderBy(c => c.Category.Name),
            "offers" => sortDescending
                ? query.OrderByDescending(c => c.Offers.Count)
                : query.OrderBy(c => c.Offers.Count),
            _ => query.OrderBy(c => c.Name)
        };

        return query;
    }

    private async Task<Dictionary<string, List<string>>> GetAvailableAttributeValues(
        IQueryable<Crops> query)
    {
        var result = new Dictionary<string, List<string>>();

        var allAttributes = await query
            .SelectMany(c => c.CropAttributes)
            .Include(ca => ca.AttributeDefinition)
            .ToListAsync();

        foreach (var attr in allAttributes.GroupBy(a => a.AttributeDefinitionId))
        {
            result[attr.Key] = attr
                .Select(a => a.Value)
                .Distinct()
                .ToList();
        }

        return result;
    }

    private async Task UpdateCropAttributesAsync(
        Crops crop,
        ICollection<CropAttributeCreateDto> newAttributes)
    {
        // Validate new attributes
        await ValidateCropAttributesAsync(newAttributes, crop.Category);

        // Remove old attributes
        var attributesToRemove = crop.CropAttributes
            .Where(ca => !newAttributes.Any(na => na.AttributeDefinitionId == ca.AttributeDefinitionId))
            .ToList();

        foreach (var attr in attributesToRemove)
        {
            crop.CropAttributes.Remove(attr);
        }

        // Update existing and add new attributes
        foreach (var newAttr in newAttributes)
        {
            var existingAttr = crop.CropAttributes
                .FirstOrDefault(ca => ca.AttributeDefinitionId == newAttr.AttributeDefinitionId);

            if (existingAttr != null)
            {
                existingAttr.Value = newAttr.Value;
            }
            else
            {
                crop.CropAttributes.Add(new CropAttributes
                {
                    Id = Guid.NewGuid().ToString(),
                    CropId = crop.Id,
                    Crop = crop,
                    AttributeDefinitionId = newAttr.AttributeDefinitionId,
                    Value = newAttr.Value,
                    AttributeDefinition = crop.Category.AttributeDefinitions
                        .First(ad => ad.Id == newAttr.AttributeDefinitionId)
                });
            }
        }
    }
}