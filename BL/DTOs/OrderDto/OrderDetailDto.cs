using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.OrderDto;

public class OrderDetailDto
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Price { get; set; }
    public PaymentType PaymentType { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderOfferDetailDto> OrderOffers { get; set; } = new();
}