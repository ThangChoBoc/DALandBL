using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZelnyTrh.EF.DAL.Entities;

public class AttributeDefinition : IEntity<string>
{
    [Key]
    public required string Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    public required string DataType { get; set; }

    public required bool IsRequired { get; set; }
    public string? ValidationRule { get; set; }
    public string? Unit { get; set; }

    public required string CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual required CropCategories Category { get; set; }

    // Navigation property for attribute values
    public virtual ICollection<CropAttributes> CropAttributes { get; set; } = [];
}