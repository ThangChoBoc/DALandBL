using ZelnyTrh.EF.BL.DTOs.OrderDto;

namespace ZelnyTrh.EF.BL.Services.OrderService;

public interface IOrderService
{
    Task<OrderReadDto> CreateOrderAsync(OrderCreateDto dto);
    Task<OrderReadDto> GetOrderByIdAsync(string id);
    Task<IEnumerable<OrderDetailDto>> GetUserOrdersAsync(string userId);
    Task<OrderReadDto> UpdateOrderStatusAsync(string id, OrderUpdateDto dto);
    Task<bool> ValidateOrderAsync(OrderCreateDto dto);
}