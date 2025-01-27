using ZelnyTrh.EF.BL.DTOs.AttributesDto;

namespace ZelnyTrh.EF.BL.DTOs.CropDTO;

public class CropListDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string CategoryName { get; set; }
    public int ActiveOffersCount { get; set; }
    public List<CategoryWithAttributesDto> MainAttributes { get; set; } = [];
}