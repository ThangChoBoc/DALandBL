using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZelnyTrh.EF.DAL.Entities
{
    public class Reviews : IEntity<string>
    {
        [Key]
        public required string Id { get; set; }
        public int Rating { get; set; }
        public string ReviewDescription { get; set; } = "None";

        public required string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public required ApplicationUser User { get; set; }

        public required string OfferId { get; set; }
        [ForeignKey(nameof(OfferId))]
        public required Offers Offer { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
