using ZelnyTrh.EF.BL.DTOs.OfferDTO;

namespace ZelnyTrh.EF.BL.DTOs.UserDto;

public class UserDetailDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public ICollection<string> Roles { get; set; } = [];
    public DateTime RegisteredAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool TwoFactorEnabled { get; set; }

    // Stats and related data
    public int TotalOffers { get; set; }
    public int ActiveOrders { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<OfferListDto> RecentOffers { get; set; } = [];
}