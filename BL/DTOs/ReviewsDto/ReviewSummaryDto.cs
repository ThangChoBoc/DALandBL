namespace ZelnyTrh.EF.BL.DTOs.ReviewsDto;

public class ReviewSummaryDto
{
    public string OfferId { get; set; } = string.Empty;
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = [];
    public List<ReviewReadDto> RecentReviews { get; set; } = [];
}