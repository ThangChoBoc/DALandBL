using ZelnyTrh.EF.BL.DTOs.AttributesDto;

namespace ZelnyTrh.EF.BL.DTOs.CropDTO;
public class CropReadDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public List<CropAttributeReadDto> Attributes { get; set; } = [];
    public List<string> CategoryPath { get; set; } = [];
}
