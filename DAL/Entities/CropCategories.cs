using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.DAL.Entities;

public class CropCategories : IEntity<string>
{
    [Key]
    public required string Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    public CropCategoryStatus CropCategoryStatus { get; set; }

    public string? ParentCategoryId { get; set; }

    [ForeignKey(nameof(ParentCategoryId))]
    public virtual CropCategories? ParentCategory { get; set; }

    public virtual ICollection<CropCategories> ChildCategories { get; set; } = [];

    public virtual ICollection<AttributeDefinition> AttributeDefinitions { get; set; } = [];

    public virtual ICollection<Crops> Crops { get; set; } = [];
}
