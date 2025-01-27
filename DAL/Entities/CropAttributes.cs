using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZelnyTrh.EF.DAL.Entities;

public class CropAttributes : IEntity<string>
{
    [Key]
    public required string Id { get; set; }

    public required string CropId { get; set; }

    [ForeignKey(nameof(CropId))]
    public virtual required Crops Crop { get; set; }

    public required string AttributeDefinitionId { get; set; }

    [ForeignKey(nameof(AttributeDefinitionId))]
    public virtual required AttributeDefinition AttributeDefinition { get; set; }

    [Required]
    public required string Value { get; set; }
}