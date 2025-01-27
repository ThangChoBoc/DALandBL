using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZelnyTrh.EF.DAL.Entities;
public class OrderOffers : IEntity<string> 
{
    [Key]
    public required string Id { get; set; }
    public required string OrderId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public virtual required Orders Order { get; set; }
    public required string OfferId { get; set; }
    [ForeignKey(nameof(OfferId))]
    public virtual required Offers Offer { get; set; }
}
