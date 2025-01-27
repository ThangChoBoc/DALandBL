using ZelnyTrh.EF.BL.DTOs.OrderOfferDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.OrderDto;

public class OrderCreateDto
{
    public required string UserId { get; set; }
    public required decimal Amount { get; set; }
    public required double Price { get; set; }
    public required PaymentType PaymentType { get; set; }
    public required OrderStatus OrderStatus { get; set; }

    // Can get OrderId and OfferId
    public required ICollection<OrderOfferCreateDto> OrderOffers { get; set; } = [];

}