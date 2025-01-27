using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.CropCategoriesDto;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.BL.Mappers;

public class CropCategoryMappingProfile : Profile
{
    public CropCategoryMappingProfile()
    {
        // Entity -> List DTO
        CreateMap<CropCategories, CropCategoryListDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.CropsCount, opt => opt.MapFrom(src => src.Crops.Count))
            .ForMember(dest => dest.OffersCount, opt => opt.MapFrom(src => src.Crops.Sum(c => c.Offers.Count)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CropCategoryStatus))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Entity -> Read DTO
        CreateMap<CropCategories, CropCategoryReadDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.CropsCount, opt => opt.MapFrom(src => src.Crops.Count))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CropCategoryStatus));

        // Create DTO -> Entity
        CreateMap<CropCategoryCreateDto, CropCategories>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
            .ForMember(dest => dest.CropCategoryStatus, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.AttributeDefinitions, opt => opt.Ignore())
            .ForMember(dest => dest.Crops, opt => opt.Ignore())
            .ForMember(dest => dest.ChildCategories, opt => opt.Ignore());

        // Update DTO -> Entity
        CreateMap<CropCategoryUpdateDto, CropCategories>()
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
            .ForMember(dest => dest.CropCategoryStatus, opt => opt.Ignore())
            .ForMember(dest => dest.AttributeDefinitions, opt => opt.Ignore())
            .ForMember(dest => dest.Crops, opt => opt.Ignore())
            .ForMember(dest => dest.ChildCategories, opt => opt.Ignore());
    }
}