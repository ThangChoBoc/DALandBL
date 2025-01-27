using ZelnyTrh.EF.BL.DTOs.AttributesDto;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.CropCategoriesDto;

public class CropCategoryReadDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public CropCategoryStatus Status { get; set; }
    public List<AttributeDefinitionReadDto> AttributeDefinitions { get; set; } = [];
    public List<CropCategoryReadDto> ChildCategories { get; set; } = [];
    public int CropsCount { get; set; }
}