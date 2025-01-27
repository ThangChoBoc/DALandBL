using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZelnyTrh.EF.DAL.Entities;
public class SelfPickups : IEntity<string>
{
    [Key]
    public required string Id { get; set; }
    public required string Location { get; set; }
    public DateTime Starting { get; set; }
    public DateTime Ending { get; set; }

    // Navigation property to Offer
    public required string OfferId { get; set; }
    [ForeignKey(nameof(OfferId))]
    public virtual required Offers Offer { get; set; }

    // Collection of registrations
    public virtual ICollection<SelfPickupRegistrations> Registrations { get; set; } = [];
}