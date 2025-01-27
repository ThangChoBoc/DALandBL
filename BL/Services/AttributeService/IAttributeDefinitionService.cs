using ZelnyTrh.EF.BL.DTOs.AttributesDto;
namespace ZelnyTrh.EF.BL.Services.AttributeService;

public interface IAttributeDefinitionService
{
    Task<AttributeDefinitionReadDto> CreateAttributeDefinitionAsync(AttributeDefinitionCreateDto dto);
    Task<AttributeDefinitionReadDto> GetAttributeDefinitionByIdAsync(string id);
    Task<IEnumerable<AttributeDefinitionReadDto>> GetAllAttributeDefinitionsAsync();
    Task<AttributeDefinitionReadDto> UpdateAttributeDefinitionAsync(AttributeDefinitionUpdateDto dto);
    Task DeleteAttributeDefinitionAsync(string id);
}