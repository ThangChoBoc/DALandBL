using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.ReviewsDto;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.BL.Mappers;
public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        // Entity -> Read DTO
        CreateMap<Reviews, ReviewReadDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.OfferName, opt => opt.MapFrom(src => src.Offer.Name));

        // Create DTO -> Entity
        CreateMap<ReviewCreateDto, Reviews>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Offer, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Update DTO -> Entity
        CreateMap<ReviewUpdateDto, Reviews>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.OfferId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Offer, opt => opt.Ignore());
    }
}