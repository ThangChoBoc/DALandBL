using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.OfferDTO;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.BL.Mappers
{
    public class OfferMappingProfile : Profile
    {
        public OfferMappingProfile()
        {
            CreateMap<Offers, OfferListDto>()
                .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.CropName, opt => opt.MapFrom(src => src.Crop.Name != null ? src.Crop.Name : "Unknown"))
                .ForMember(dest => dest.AverageRating, opt =>
                    opt.MapFrom(src => src.User.Review.Any() ?
                        src.User.Review.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.ReviewsCount, opt =>
                    opt.MapFrom(src => src.User.Review.Count));

            CreateMap<Offers, OfferReadDto>();

            CreateMap<OfferCreateDto, Offers>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Crop, opt => opt.Ignore())
                .ForMember(dest => dest.OrderOffers, opt => opt.Ignore())
                .ForMember(dest => dest.SelfPickups, opt => opt.Ignore());

            CreateMap<OfferUpdateDto, Offers>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CropId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Crop, opt => opt.Ignore())
                .ForMember(dest => dest.OrderOffers, opt => opt.Ignore())
                .ForMember(dest => dest.SelfPickups, opt => opt.Ignore());
        }
    }
}