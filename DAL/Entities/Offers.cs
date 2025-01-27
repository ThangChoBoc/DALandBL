using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.DAL.Entities;

public class Offers : IEntity<string>
{
    [Key]
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required double Price { get; set; }
    public required string Currency { get; set; }
    public required decimal Amount { get; set; }
    public required decimal UnitsAvailable { get; set; }
    public required string Origin { get; set; }
    public required OfferType OfferType { get; set; }
    public required string UserId { get; set; }
    public virtual ICollection<SelfPickups> SelfPickups { get; set; } = [];

    // NAV properties
    [ForeignKey(nameof(UserId))]
    public virtual required ApplicationUser User { get; set; }

    // Foreign key to Crops
    public required string CropId { get; set; }

    [ForeignKey(nameof(CropId))]
    public virtual required Crops Crop { get; set; }

    // m-m relationship with Orders
    public virtual ICollection<OrderOffers> OrderOffers { get; set; } = [];
}