namespace ZelnyTrh.EF.BL.DTOs.OrderDto;

public class OrderOfferDetailDto
{
    public required string OfferId { get; set; }
    public required string OfferName { get; set; }
    public required string FarmerName { get; set; }
    public required double Price { get; set; }
    public required decimal Amount { get; set; }
    public required string CropName { get; set; }
}