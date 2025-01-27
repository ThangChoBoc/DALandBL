using ZelnyTrh.EF.BL.DTOs.AttributesDto;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.CropCategoriesDto;

public class CropCategoryCreateDto
{
    public required string Name { get; set; }
    public string? ParentCategoryId { get; set; }
    public required CropCategoryStatus Status { get; set; } = CropCategoryStatus.Pending;
    public string? ProposedByUserId { get; set; }  // No longer required since set by controller
    public List<AttributeDefinitionReadDto> AttributeDefinitions { get; set; } = [];
}