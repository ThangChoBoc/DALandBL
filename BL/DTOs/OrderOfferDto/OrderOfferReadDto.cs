using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.OrderOfferDto;

public class OrderOfferReadDto
{
    public required string Id { get; set; }
    public required string OrderId { get; set; }
    public required string OfferId { get; set; }

    // Additional properties from related entities
    public required string OfferName { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public PaymentType PaymentType { get; set; }

    // New properties for Farmer information
    public required string FarmerId { get; set; }
    public required string FarmerName { get; set; }

    public string BuyerName { get; init; } = "Unknown Buyer";
    public string BuyerId { get; init; } = "Unknown BuyerId";
}
