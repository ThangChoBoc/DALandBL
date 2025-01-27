namespace ZelnyTrh.EF.BL.DTOs.CropDTO;

public class CategoryWithAttributesDto
{
    public required string CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public Dictionary<string, string> AttributeValues { get; set; } = [];
}
