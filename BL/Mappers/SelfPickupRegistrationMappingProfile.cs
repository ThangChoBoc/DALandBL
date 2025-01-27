using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.SelfPickupRegistrationDto;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.BL.Mappers;

public class SelfPickupRegistrationMappingProfile : Profile
{
    public SelfPickupRegistrationMappingProfile()
    {
        // SelfPickupRegistrations -> SelfPickupRegistrationReadDto
        CreateMap<SelfPickupRegistrations, SelfPickupRegistrationReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SelfPickupId, opt => opt.MapFrom(src => src.SelfPickupId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.RegisteredAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // SelfPickupRegistrationCreateDto -> SelfPickupRegistrations
        CreateMap<SelfPickupRegistrationCreateDto, SelfPickupRegistrations>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.SelfPickupId, opt => opt.MapFrom(src => src.SelfPickupId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.SelfPickup, opt => opt.Ignore());
    }
}