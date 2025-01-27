using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.SelfPickupDto;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.BL.Mappers;

public class SelfPickupMappingProfile : Profile
{
    public SelfPickupMappingProfile()
    {
        // SelfPickups -> SelfPickupReadDto
        CreateMap<SelfPickups, SelfPickupReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.Starting, opt => opt.MapFrom(src => src.Starting))
            .ForMember(dest => dest.Ending, opt => opt.MapFrom(src => src.Ending))
            .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.OfferId))
            .ForMember(dest => dest.OfferName, opt => opt.MapFrom(src => src.Offer.Name))
            .ForMember(dest => dest.FarmerId, opt => opt.MapFrom(src => src.Offer.UserId))
            .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Offer.User.Name))
            .ForMember(dest => dest.RegisteredUsersCount, opt => opt.MapFrom(src => src.Registrations.Count));

        // SelfPickupCreateDto -> SelfPickups
        CreateMap<SelfPickupCreateDto, SelfPickups>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.Starting, opt => opt.MapFrom(src => src.Starting))
            .ForMember(dest => dest.Ending, opt => opt.MapFrom(src => src.Ending))
            .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.OfferId))
            .ForMember(dest => dest.Offer, opt => opt.Ignore())
            .ForMember(dest => dest.Registrations, opt => opt.MapFrom(src => new List<SelfPickupRegistrations>()));
    }
}