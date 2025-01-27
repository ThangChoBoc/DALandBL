using ZelnyTrh.EF.BL.DTOs.AttributesDto;

namespace ZelnyTrh.EF.BL.DTOs.CropCategoriesDto;

public class CropCategoryUpdateDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? ParentCategoryId { get; set; }
    public List<AttributeDefinitionReadDto> AttributeDefinitions { get; set; } = [];
}