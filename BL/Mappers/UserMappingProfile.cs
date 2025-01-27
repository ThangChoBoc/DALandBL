using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.UserDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Enums;
namespace ZelnyTrh.EF.BL.Mappers;
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ApplicationUser, UserDetailDto>()
            .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.TotalOffers, opt => opt.MapFrom(src => src.Offer.Count))
            .ForMember(dest => dest.ActiveOrders, opt =>
                opt.MapFrom(src => src.Orders.Count(o => o.OrderStatus != OrderStatus.Completed)))
            .ForMember(dest => dest.AverageRating, opt =>
                opt.MapFrom(src => src.Review.Any() ? src.Review.Average(r => r.Rating) : 0))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Review.Count))
            .ForMember(dest => dest.RecentOffers, opt => opt.MapFrom(src => src.Offer.Take(5)))
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.RegisteredAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.LastLogin, opt => opt.Ignore());

        CreateMap<ApplicationUser, UserListDto>()
            .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ForMember(dest => dest.RegisteredAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UserCreateDto, ApplicationUser>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Offer, opt => opt.Ignore())
            .ForMember(dest => dest.Review, opt => opt.Ignore())
            .ForMember(dest => dest.SelfPickupRegistrations, opt => opt.Ignore())
            .ForMember(dest => dest.Claims, opt => opt.Ignore())
            .ForMember(dest => dest.Logins, opt => opt.Ignore())
            .ForMember(dest => dest.Tokens, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        CreateMap<UserUpdateDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Offer, opt => opt.Ignore())
            .ForMember(dest => dest.Review, opt => opt.Ignore())
            .ForMember(dest => dest.SelfPickupRegistrations, opt => opt.Ignore())
            .ForMember(dest => dest.Claims, opt => opt.Ignore())
            .ForMember(dest => dest.Logins, opt => opt.Ignore())
            .ForMember(dest => dest.Tokens, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());
    }
}