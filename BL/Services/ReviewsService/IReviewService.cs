using ZelnyTrh.EF.BL.DTOs.ReviewsDto;

namespace ZelnyTrh.EF.BL.Services.ReviewsService;

public interface IReviewService
{
    Task<ReviewReadDto> CreateReviewAsync(ReviewCreateDto dto, string userId);
    Task<IEnumerable<ReviewReadDto>> GetOfferReviewsAsync(string offerId);
    Task<IEnumerable<ReviewReadDto>> GetUserReviewsAsync(string userId);
    Task<double> GetAverageRatingForOfferAsync(string offerId);
    Task<IDictionary<string, double>> GetFarmerAverageRatingsAsync(string farmerId);
    Task<ReviewSummaryDto> GetOfferReviewSummaryAsync(string offerId);
    Task<bool> UpdateReviewAsync(string reviewId, ReviewUpdateDto dto, string userId);
    Task<bool> DeleteReviewAsync(string reviewId, string userId);
}