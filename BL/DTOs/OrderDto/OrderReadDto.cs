using System.Collections.Generic;
using ZelnyTrh.EF.BL.DTOs.OrderOfferDto;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.OrderDto
{
    public class OrderReadDto
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public decimal Amount { get; set; } // Total amount of the order
        public double Price { get; set; }
        public PaymentType PaymentType { get; set; }
        public OrderStatus OrderStatus { get; set; }

        // Use OrderOfferReadDto instead of OrderOffers entity
        public ICollection<OrderOfferReadDto> OrderOffers { get; set; } = [];
    }
}