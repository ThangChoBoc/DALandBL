using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.DAL.Entities
{
    public class Orders : IEntity<string>
    {
        [Key]
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public decimal Amount { get; set; }
        public double Price { get; set; }
        public PaymentType PaymentType { get; set; }
        public OrderStatus OrderStatus { get; set; }

        // NAV properties
        [ForeignKey(nameof(UserId))]
        public virtual required ApplicationUser User { get; set; }

        // Keep the many-to-many relationship through OrderOffers
        public virtual ICollection<OrderOffers> OrderOffers { get; set; } = [];
    }
}