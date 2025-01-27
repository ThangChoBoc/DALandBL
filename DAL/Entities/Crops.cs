using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZelnyTrh.EF.DAL.Entities;

public class Crops : IEntity<string>
{
    [Key]
    public required string Id { get; set; }
    public required string Name { get; set; }
    // Single category relationship
    public required string CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual required CropCategories Category { get; set; }

    // Attributes for this crop instance
    public virtual ICollection<CropAttributes> CropAttributes { get; set; } = [];

    // Navigation property to Offers
    public virtual ICollection<Offers> Offers { get; set; } = [];
}