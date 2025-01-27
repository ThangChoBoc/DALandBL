namespace ZelnyTrh.EF.BL.DTOs.AttributesDto;
public class CropAttributeCreateDto
{
    public required string CropId { get; set; }
    public required string AttributeDefinitionId { get; set; }
    public required string Value { get; set; }
}