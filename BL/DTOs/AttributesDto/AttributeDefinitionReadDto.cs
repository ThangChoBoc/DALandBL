namespace ZelnyTrh.EF.BL.DTOs.AttributesDto;
public class AttributeDefinitionReadDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string DataType { get; set; }
    public required bool IsRequired { get; set; }
    public string? ValidationRule { get; set; }
    public string? Unit { get; set; }
    public required string CategoryId { get; set; }
    public required string CategoryName { get; set; }
}