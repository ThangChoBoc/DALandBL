using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.CropCategoriesDto;

public class CropCategoryListDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? ParentCategoryName { get; set; }
    public CropCategoryStatus Status { get; set; }
    public int CropsCount { get; set; }
    public int OffersCount { get; set; }
    public DateTime CreatedAt { get; set; }
}