using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ZelnyTrh.EF.BL.DTOs.OrderDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Enums;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.OrderService;

public class OrderService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : IOrderService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    public async Task<OrderReadDto> CreateOrderAsync(OrderCreateDto dto)
    {
        // Validate user exists
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {dto.UserId} not found");

        // Start tracking all offers first for locking
        var offerIds = dto.OrderOffers.Select(oo => oo.OfferId).ToList();
        var offers = await _unitOfWork.GetRepository<Offers>()
            .GetByConditionAsync(o => offerIds.Contains(o.Id));

        // Validate all offers exist and are available
        var availableOffers = offers.ToDictionary(
            o => o.Id,
            o => o);

        foreach (var orderOffer in dto.OrderOffers)
        {
            if (!availableOffers.TryGetValue(orderOffer.OfferId, out var offer))
            {
                throw new InvalidOperationException($"Offer with ID {orderOffer.OfferId} not found");
            }

            if (offer.UnitsAvailable <= 0)
            {
                throw new InvalidOperationException($"Offer {offer.Name} is out of stock");
            }
        }

        // Create the order
        var order = new Orders
        {
            Id = Guid.NewGuid().ToString(),
            UserId = dto.UserId,
            Amount = dto.Amount,
            Price = dto.Price,
            PaymentType = dto.PaymentType,
            OrderStatus = OrderStatus.Started,
            User = user,
            OrderOffers = new List<OrderOffers>()
        };

        // Create order offers and update stock
        foreach (var orderOfferDto in dto.OrderOffers)
        {
            var offer = availableOffers[orderOfferDto.OfferId];

            // Create the order-offer relationship
            var orderOffer = new OrderOffers
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = order.Id,
                OfferId = offer.Id,
                Order = order,
                Offer = offer
            };
            order.OrderOffers.Add(orderOffer);

            // Update offer stock
            offer.UnitsAvailable = (decimal)(offer.UnitsAvailable -  dto.Amount);
            await _unitOfWork.GetRepository<Offers>().UpdateAsync(offer);
        }

        // Save order
        await _unitOfWork.GetRepository<Orders>().InsertAsync(order);
        await _unitOfWork.CommitAsync();

        // Return mapped DTO
        return _mapper.Map<OrderReadDto>(order);
    }

    public async Task<OrderReadDto> GetOrderByIdAsync(string id)
    {
        var order = await _unitOfWork.GetRepository<Orders>().GetByIdAsync(id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found");

        return _mapper.Map<OrderReadDto>(order);
    }

    public async Task<IEnumerable<OrderDetailDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _unitOfWork.GetRepository<Orders>()
            .GetByConditionAsync(o => o.UserId == userId);

        return orders.Select(o => _mapper.Map<OrderDetailDto>(o));
    }

    public async Task<OrderReadDto> UpdateOrderStatusAsync(string id, OrderUpdateDto dto)
    {
        var order = await _unitOfWork.GetRepository<Orders>().GetByIdAsync(id);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {id} not found");

        // Validate status transition
        if (!IsValidStatusTransition(order.OrderStatus, dto.OrderStatus))
            throw new InvalidOperationException($"Invalid status transition from {order.OrderStatus} to {dto.OrderStatus}");

        order.OrderStatus = dto.OrderStatus;
        await _unitOfWork.GetRepository<Orders>().UpdateAsync(order);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<OrderReadDto>(order);
    }

    public async Task<bool> ValidateOrderAsync(OrderCreateDto dto)
    {
        try
        {
            // Check if user exists
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return false;

            // Validate each offer in the order
            foreach (var orderOffer in dto.OrderOffers)
            {
                var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(orderOffer.OfferId);
                if (offer.UnitsAvailable <= 0)
                    return false;
            }

            // Validate payment type
            if (!Enum.IsDefined(typeof(PaymentType), dto.PaymentType))
                return false;

            // Validate amount is positive
            if (dto.Amount <= 0)
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
    private async Task<bool> IsUserAdminAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, "Administrator");
    }

    private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (OrderStatus.Started, OrderStatus.InTransit) => true,
            (OrderStatus.Started, OrderStatus.AwaitingCollection) => true,
            (OrderStatus.InTransit, OrderStatus.Completed) => true,
            (OrderStatus.AwaitingCollection, OrderStatus.Completed) => true,
            _ => false
        };
    }
}
