using ZelnyTrh.EF.BL.DTOs.OfferDTO;

namespace ZelnyTrh.EF.BL.Services.OfferService;

public interface IOfferService
{
    // Basic CRUD operations
    Task<OfferReadDto> CreateOfferAsync(OfferCreateDto offerCreateDto);
    Task<OfferReadDto> GetOfferByIdAsync(string id);
    Task<IEnumerable<OfferListDto>> GetAllOffersAsync();
    Task<OfferReadDto> UpdateOfferAsync(OfferUpdateDto offerUpdateDto, string currentUserId);
    Task DeleteOfferAsync(string id);

    // Additional query methods
    Task<IEnumerable<OfferListDto>> GetOffersByUserAsync(string userId);
    Task<IEnumerable<OfferListDto>> GetOffersByCategoryAsync(string categoryId);
    Task<IEnumerable<OfferListDto>> GetActiveOffersAsync();
    Task<IEnumerable<OfferListDto>> SearchOffersAsync(OfferSearchDto searchDto);
}
