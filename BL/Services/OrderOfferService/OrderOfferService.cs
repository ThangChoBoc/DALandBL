using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.OrderOfferDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Enums;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.OrderOfferService;

/*
When a user places an order with multiple items (offers)
When you need to track which offers are part of which orders
When you need to calculate order totals or track order history
When you need to check what orders contain a particular offer
*/

public class OrderOfferService : IOrderOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderOfferService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderOfferReadDto> CreateOrderOfferAsync(OrderOfferCreateDto dto)
    {
        // Verify order exists
        var orders = await _unitOfWork.GetRepository<Orders>()
            .GetByConditionAsync(o => o.Id == dto.OrderId);
        var order = orders.FirstOrDefault();

        if (order == null)
            throw new KeyNotFoundException($"Order with ID {dto.OrderId} not found");

        // Verify offer exists and is available
        var offers = await _unitOfWork.GetRepository<Offers>()
            .GetByConditionAsync(o => o.Id == dto.OfferId);
        var offer = offers.FirstOrDefault();

        if (offer == null)
            throw new KeyNotFoundException($"Offer with ID {dto.OfferId} not found");

        // Check if this combination already exists
        var existingOrderOffers = await _unitOfWork.GetRepository<OrderOffers>()
            .GetByConditionAsync(oo => oo.OrderId == dto.OrderId && oo.OfferId == dto.OfferId);

        if (existingOrderOffers.Any())
            throw new InvalidOperationException("This offer is already added to the order");

        // Create new order-offer relationship
        var orderOffer = new OrderOffers
        {
            Id = Guid.NewGuid().ToString(),
            OrderId = dto.OrderId,
            OfferId = dto.OfferId,
            Order = order,
            Offer = offer
        };

        await _unitOfWork.GetRepository<OrderOffers>().InsertAsync(orderOffer);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<OrderOfferReadDto>(orderOffer);
    }

    public async Task<OrderOfferReadDto> GetOrderOfferByIdAsync(string id)
    {
        var orderOffers = await _unitOfWork.GetRepository<OrderOffers>()
            .GetByConditionAsync(oo => oo.Id == id);
        var orderOffer = orderOffers.FirstOrDefault();

        if (orderOffer == null)
            throw new KeyNotFoundException($"OrderOffer with ID {id} not found");

        return _mapper.Map<OrderOfferReadDto>(orderOffer);
    }

    public async Task<IEnumerable<OrderOfferReadDto>> GetOrderOffersByOrderIdAsync(string orderId)
    {
        var orderOffers = await _unitOfWork.GetRepository<OrderOffers>()
            .GetByConditionAsync(oo => oo.OrderId == orderId);

        return orderOffers.Select(oo => _mapper.Map<OrderOfferReadDto>(oo));
    }

    public async Task<IEnumerable<OrderOfferReadDto>> GetOrderOffersByOfferIdAsync(string offerId)
    {
        var orderOffers = await _unitOfWork.GetRepository<OrderOffers>()
            .GetByConditionAsync(oo => oo.OfferId == offerId);

        return orderOffers.Select(oo => _mapper.Map<OrderOfferReadDto>(oo));
    }

    public async Task DeleteOrderOfferAsync(string id)
    {
        var orderOffers = await _unitOfWork.GetRepository<OrderOffers>()
            .GetByConditionAsync(oo => oo.Id == id);
        var orderOffer = orderOffers.FirstOrDefault();

        if (orderOffer == null)
            throw new KeyNotFoundException($"OrderOffer with ID {id} not found");

        // You might want to add additional validation here
        // For example, checking if the order is in a state where removing offers is allowed

        await _unitOfWork.GetRepository<OrderOffers>().DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<OrderOfferReadDto>> GetOrderOffersByUserIdAsync(string userId)
    {
        // Step 1: Fetch all user orders in one call
        var userOrders = await _unitOfWork.GetRepository<Orders>()
            .GetByConditionAsync(o => o.UserId == userId);

        if (!userOrders.Any())
        {
            return Enumerable.Empty<OrderOfferReadDto>();
        }

        var orderIds = userOrders.Select(o => o.Id).ToList();

        // Step 2: Fetch all OrderOffers linked to the user's orders
        var orderOffers = await _unitOfWork.GetRepository<OrderOffers>()
            .GetByConditionAsync(oo => orderIds.Contains(oo.OrderId));

        if (!orderOffers.Any())
        {
            return Enumerable.Empty<OrderOfferReadDto>();
        }

        var offerIds = orderOffers.Select(oo => oo.OfferId).Distinct().ToList();

        // Step 3: Fetch all Offers in one call
        var offers = await _unitOfWork.GetRepository<Offers>()
            .GetByConditionAsync(o => offerIds.Contains(o.Id));

        if (!offers.Any())
        {
            return Enumerable.Empty<OrderOfferReadDto>();
        }

        var userIds = offers.Select(o => o.UserId).Distinct().ToList();

        // Step 4: Fetch all Farmers (Users) in one call
        var farmers = await _unitOfWork.GetRepository<ApplicationUser>()
            .GetByConditionAsync(u => userIds.Contains(u.Id));

        // Step 5: Perform in-memory joins and build the DTO
        var result = orderOffers.Select(orderOffer =>
        {
            var offer = offers.FirstOrDefault(o => o.Id == orderOffer.OfferId);
            var order = userOrders.FirstOrDefault(o => o.Id == orderOffer.OrderId);
            var farmer = farmers.FirstOrDefault(u => u.Id == offer?.UserId);

            return new OrderOfferReadDto
            {
                Id = orderOffer.Id,
                OrderId = orderOffer.OrderId,
                OfferId = orderOffer.OfferId,
                OfferName = offer?.Name ?? "Unknown Offer",
                Price = (int)offer.Price,
                Amount = order?.Amount ?? 0,
                OrderStatus = order?.OrderStatus ?? OrderStatus.Started,
                PaymentType = order?.PaymentType ?? PaymentType.Cash,
                FarmerId = farmer?.Id ?? "Unknown Farmer ID",
                FarmerName = farmer?.Name ?? "Unknown Farmer"
            };
        });

        return result;
    }
}