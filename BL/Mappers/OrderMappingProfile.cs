using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.OrderDto;
using ZelnyTrh.EF.BL.DTOs.OrderOfferDto;
using ZelnyTrh.EF.DAL.Entities;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // Order -> OrderReadDto
        CreateMap<Orders, OrderReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
            .ForMember(dest => dest.OrderOffers, opt => opt.MapFrom(src => src.OrderOffers));

        // OrderCreateDto -> Orders
        CreateMap<OrderCreateDto, Orders>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.OrderOffers, opt => opt.Ignore());

        // Orders -> OrderDetailDto
        CreateMap<Orders, OrderDetailDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.OrderOffers, opt => opt.MapFrom(src => src.OrderOffers));

        // Combined OrderOffers -> OrderOfferReadDto mapping
        CreateMap<OrderOffers, OrderOfferReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.OfferId))
            .ForMember(dest => dest.OfferName, opt => opt.MapFrom(src => src.Offer.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Order.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Order.Amount))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Order.OrderStatus))
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.Order.PaymentType))
            .ForMember(dest => dest.FarmerId, opt => opt.MapFrom(src => src.Offer.UserId))
            .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Offer.User.UserName))
            .ForMember(dest => dest.BuyerName, opt => opt.Ignore())
            .ForMember(dest => dest.BuyerId, opt => opt.Ignore());
        // OrderOffers -> OrderOfferDetailDto
        CreateMap<OrderOffers, OrderOfferDetailDto>()
            .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.OfferId))
            .ForMember(dest => dest.OfferName, opt => opt.MapFrom(src => src.Offer.Name))
            .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Offer.User.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Offer.Price))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Offer.Amount))
            .ForMember(dest => dest.CropName, opt => opt.MapFrom(src => src.Offer.Crop.Name));

        // OrderOfferCreateDto -> OrderOffers
        CreateMap<OrderOfferCreateDto, OrderOffers>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.OfferId))
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Offer, opt => opt.Ignore());
    }
}