namespace ZelnyTrh.EF.BL.DTOs.AttributesDto;
public class CropAttributeReadDto
{
    public required string Id { get; set; }
    public required string CropId { get; set; }
    public required string CropName { get; set; }
    public required string AttributeDefinitionId { get; set; }
    public required string AttributeName { get; set; }
    public required string Value { get; set; }
}