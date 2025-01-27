using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ZelnyTrh.EF.BL.DTOs.ReviewsDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.ReviewsService;

public class ReviewService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    UserManager<ApplicationUser> userManager) : IReviewService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<ReviewReadDto> CreateReviewAsync(ReviewCreateDto dto, string userId)
    {
        // Verify the user has purchased this offer
        var hasOrdered = await VerifyUserPurchaseAsync(userId, dto.OfferId);
        if (!hasOrdered)
            throw new InvalidOperationException("User has not purchased this offer");

        var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(dto.OfferId);
        var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(userId);

        var review = new Reviews
        {
            Id = Guid.NewGuid().ToString(),
            Rating = dto.Rating,
            ReviewDescription = dto.ReviewDescription,
            UserId = userId,
            OfferId = dto.OfferId,
            User = user,
            Offer = offer,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.GetRepository<Reviews>().InsertAsync(review);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<ReviewReadDto>(review);
    }

    public async Task<IEnumerable<ReviewReadDto>> GetOfferReviewsAsync(string offerId)
    {
        var reviewRepository = _unitOfWork.GetRepository<Reviews>();
        var reviews = await reviewRepository.GetByConditionAsync(r => r.OfferId == offerId);

        var result = new List<ReviewReadDto>();
        foreach (var review in reviews)
        {
            var dto = await GetReviewWithDetailsAsync(review.Id);
            result.Add(dto);
        }

        // Order by most recent first
        return result.OrderByDescending(r => r.CreatedAt);
    }

    public async Task<IEnumerable<ReviewReadDto>> GetUserReviewsAsync(string userId)
    {
        var reviewRepository = _unitOfWork.GetRepository<Reviews>();
        var reviews = await reviewRepository.GetByConditionAsync(r => r.UserId == userId);

        var result = new List<ReviewReadDto>();
        foreach (var review in reviews)
        {
            var dto = await GetReviewWithDetailsAsync(review.Id);
            result.Add(dto);
        }

        // Order by most recent first
        return result.OrderByDescending(r => r.CreatedAt);
    }

    public async Task<double> GetAverageRatingForOfferAsync(string offerId)
    {
        var reviewRepository = _unitOfWork.GetRepository<Reviews>();
        var reviews = await reviewRepository.GetByConditionAsync(r => r.OfferId == offerId);

        if (!reviews.Any())
            return 0;

        return Math.Round(reviews.Average(r => r.Rating), 1);
    }
    public async Task<ReviewSummaryDto> GetOfferReviewSummaryAsync(string offerId)
    {
        var reviewRepository = _unitOfWork.GetRepository<Reviews>();
        var reviews = await reviewRepository.GetByConditionAsync(r => r.OfferId == offerId);

        var summary = new ReviewSummaryDto
        {
            OfferId = offerId,
            TotalReviews = reviews.Count(),
            AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 0,
            RatingDistribution = new Dictionary<int, int>(),
            RecentReviews = new List<ReviewReadDto>()
        };

        // Calculate rating distribution
        for (int i = 1; i <= 5; i++)
        {
            summary.RatingDistribution[i] = reviews.Count(r => r.Rating == i);
        }

        // Get 5 most recent reviews
        var recentReviews = reviews
            .OrderByDescending(r => r.CreatedAt)
            .Take(5);

        foreach (var review in recentReviews)
        {
            var dto = await GetReviewWithDetailsAsync(review.Id);
            summary.RecentReviews.Add(dto);
        }

        return summary;
    }

    public async Task<bool> UpdateReviewAsync(string reviewId, ReviewUpdateDto dto, string userId)
    {
        var reviewRepository = _unitOfWork.GetRepository<Reviews>();
        var review = await reviewRepository.GetByIdAsync(reviewId);

        if (review == null)
            throw new KeyNotFoundException($"Review with ID {reviewId} not found");

        if (review.UserId != userId)
            throw new UnauthorizedAccessException("Cannot update another user's review");

        review.Rating = dto.Rating;
        review.ReviewDescription = dto.ReviewDescription;

        await reviewRepository.UpdateAsync(review);
        await _unitOfWork.CommitAsync();

        return true;
    }

    public async Task<bool> DeleteReviewAsync(string reviewId, string userId)
    {
        var reviewRepository = _unitOfWork.GetRepository<Reviews>();
        var review = await reviewRepository.GetByIdAsync(reviewId);

        if (review == null)
            throw new KeyNotFoundException($"Review with ID {reviewId} not found");

        if (review.UserId != userId)
            throw new UnauthorizedAccessException("Cannot delete another user's review");

        await reviewRepository.DeleteAsync(reviewId);
        await _unitOfWork.CommitAsync();

        return true;
    }

    private async Task<bool> VerifyUserPurchaseAsync(string userId, string offerId)
    {
        var orderRepository = _unitOfWork.GetRepository<Orders>();
        var orders = await orderRepository.GetByConditionAsync(o =>
            o.UserId == userId &&
            o.OrderStatus == DAL.Enums.OrderStatus.Completed);

        return orders.Any(o =>
            o.OrderOffers.Any(oo => oo.OfferId == offerId));
    }

    private async Task<Reviews?> GetExistingReview(string userId, string offerId)
    {
        var reviewRepository = _unitOfWork.GetRepository<Reviews>();
        var reviews = await reviewRepository.GetByConditionAsync(r =>
            r.UserId == userId && r.OfferId == offerId);

        return reviews.FirstOrDefault();
    }

    private async Task<ReviewReadDto> GetReviewWithDetailsAsync(string reviewId)
    {
        var reviewRepository = _unitOfWork.GetRepository<Reviews>();
        var review = await reviewRepository.GetByIdAsync(reviewId);

        if (review == null)
            throw new KeyNotFoundException($"Review with ID {reviewId} not found");

        var dto = _mapper.Map<ReviewReadDto>(review);

        // Get user details
        var user = await _userManager.FindByIdAsync(review.UserId);
        dto.UserName = user?.Name ?? "Unknown User";

        // Get offer details
        var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(review.OfferId);
        dto.OfferName = offer?.Name ?? "Unknown Offer";

        return dto;
    }

    public Task<IDictionary<string, double>> GetFarmerAverageRatingsAsync(string farmerId)
    {
        throw new NotImplementedException();
    }
}