using ZelnyTrh.EF.BL.DTOs.OrderOfferDto;

namespace ZelnyTrh.EF.BL.Services.OrderOfferService;

public interface IOrderOfferService
{
    Task<OrderOfferReadDto> CreateOrderOfferAsync(OrderOfferCreateDto dto);
    Task<OrderOfferReadDto> GetOrderOfferByIdAsync(string id);
    Task<IEnumerable<OrderOfferReadDto>> GetOrderOffersByOrderIdAsync(string orderId);
    Task<IEnumerable<OrderOfferReadDto>> GetOrderOffersByOfferIdAsync(string offerId);
    Task<IEnumerable<OrderOfferReadDto>> GetOrderOffersByUserIdAsync(string userId);
    Task DeleteOrderOfferAsync(string id);
}
