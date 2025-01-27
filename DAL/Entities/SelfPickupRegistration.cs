using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ZelnyTrh.EF.DAL.Entities;
public class SelfPickupRegistrations : IEntity<string>
{
    [Key]
    public required string Id { get; set; }

    public required string SelfPickupId { get; set; }
    [ForeignKey(nameof(SelfPickupId))]

    public required string UserId { get; set; }
    [ForeignKey(nameof(UserId))]

    // NAV
    public virtual required SelfPickups SelfPickup { get; set; }
    public virtual required ApplicationUser User { get; set; }
}