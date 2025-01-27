namespace ZelnyTrh.EF.BL.DTOs.SelfPickupDto;

public class SelfPickupReadDto
{
    public required string Id { get; set; }
    public required string Location { get; set; }
    public DateTime Starting { get; set; }
    public DateTime Ending { get; set; }
    public required string OfferId { get; set; }
    public required string OfferName { get; set; }
    public required string FarmerId { get; set; }
    public required string FarmerName { get; set; }
    public int RegisteredUsersCount { get; set; }
}