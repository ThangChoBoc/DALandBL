namespace ZelnyTrh.EF.BL.DTOs.ReviewsDto;

public class ReviewReadDto
{
    public required string Id { get; set; }
    public int? Rating { get; set; }
    public string? ReviewDescription { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string OfferId { get; set; }
    public required string OfferName { get; set; }
}