using ZelnyTrh.EF.BL.DTOs.AttributesDto;

namespace ZelnyTrh.EF.BL.DTOs.CropDTO;

public class CropUpdateDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string CategoryId { get; set; }
    public List<CropAttributeCreateDto> Attributes { get; set; } = [];
}