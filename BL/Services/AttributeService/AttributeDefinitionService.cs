using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.AttributesDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Repositories;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.AttributeService;

public class AttributeDefinitionService(IUnitOfWork unitOfWork, IMapper mapper) : IAttributeDefinitionService
{
    public async Task<AttributeDefinitionReadDto> CreateAttributeDefinitionAsync(AttributeDefinitionCreateDto dto)
    {
        // Verify category exists
        var category = await unitOfWork.GetRepository<CropCategories>().GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {dto.CategoryId} not found");

        // Check if an attribute with the same name already exists for this category
        var existingAttributes = await unitOfWork.GetRepository<AttributeDefinition>()
            .GetByConditionAsync(a => a.CategoryId == dto.CategoryId && a.Name == dto.Name);

        if (existingAttributes.Any())
            throw new InvalidOperationException($"An attribute with name '{dto.Name}' already exists in this category");

        var entity = mapper.Map<AttributeDefinition>(dto);
        await unitOfWork.GetRepository<AttributeDefinition>().InsertAsync(entity);
        await unitOfWork.CommitAsync(); // Commit the changes

        return mapper.Map<AttributeDefinitionReadDto>(entity);
    }

    public async Task<AttributeDefinitionReadDto> GetAttributeDefinitionByIdAsync(string id)
    {
        var entity = await unitOfWork.GetRepository<AttributeDefinition>().GetByIdAsync(id);
        return mapper.Map<AttributeDefinitionReadDto>(entity);
    }

    public async Task<IEnumerable<AttributeDefinitionReadDto>> GetAllAttributeDefinitionsAsync()
    {
        var entities = await unitOfWork.GetRepository<AttributeDefinition>().GetAllAsync();
        return mapper.Map<IEnumerable<AttributeDefinitionReadDto>>(entities.ToList());
    }

    public async Task<AttributeDefinitionReadDto> UpdateAttributeDefinitionAsync(AttributeDefinitionUpdateDto dto)
    {
        var attributeRepo = unitOfWork.GetRepository<AttributeDefinition>();

        var entity = await attributeRepo.GetByIdAsync(dto.Id);
        if (entity == null)
            throw new KeyNotFoundException($"Attribute definition with ID {dto.Id} not found");

        // Check if changing name would create a duplicate in the same category
        if (entity.Name != dto.Name)
        {
            var existingAttributes = await attributeRepo.GetByConditionAsync(
                a => a.CategoryId == dto.CategoryId && a.Name == dto.Name && a.Id != dto.Id);

            if (existingAttributes.Any())
                throw new InvalidOperationException($"An attribute with name '{dto.Name}' already exists in this category");
        }

        mapper.Map(dto, entity);
        await attributeRepo.UpdateAsync(entity);
        await unitOfWork.CommitAsync(); // Commit the changes

        return mapper.Map<AttributeDefinitionReadDto>(entity);
    }

    public async Task DeleteAttributeDefinitionAsync(string id)
    {
        var attributeRepo = unitOfWork.GetRepository<AttributeDefinition>();
        var cropAttributesRepo = unitOfWork.GetRepository<CropAttributes>();

        var attribute = await attributeRepo.GetByIdAsync(id);
        if (attribute == null)
            throw new KeyNotFoundException($"Attribute definition with ID {id} not found");

        try
        {
            // First, find and delete all associated crop attributes
            var cropAttributes = await cropAttributesRepo.GetByConditionAsync(
                ca => ca.AttributeDefinitionId == id);

            foreach (var cropAttribute in cropAttributes)
            {
                await cropAttributesRepo.DeleteAsync(cropAttribute.Id);
            }

            // Then delete the attribute definition itself
            await attributeRepo.DeleteAsync(id);

            // Commit all changes in a single transaction
            await unitOfWork.CommitAsync();

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to delete attribute definition. Error: {ex.Message}", ex);
        }
    }
}