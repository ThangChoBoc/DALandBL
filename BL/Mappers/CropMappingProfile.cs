using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.AttributesDto;
using ZelnyTrh.EF.BL.DTOs.CropCategoriesDto;
using ZelnyTrh.EF.BL.DTOs.CropDTO;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.BL.Mappers;
public class CropMappingProfile : Profile
{
    public CropMappingProfile()
    {
       
        // Crop mappings
        CreateMap<Crops, CropReadDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Attributes,
                opt => opt.MapFrom(src => src.CropAttributes))
            .ForMember(dest => dest.CategoryPath,
                opt => opt.MapFrom((src, _, _, context) => GetCategoryPath(src.Category)))
            .ReverseMap();

        CreateMap<CropCreateDto, Crops>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CropAttributes, opt => opt.MapFrom(src => src.Attributes))
            .ForMember(dest => dest.Offers, opt => opt.Ignore());

        CreateMap<CropUpdateDto, Crops>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.CropAttributes, opt => opt.MapFrom(src => src.Attributes))
            .ForMember(dest => dest.Offers, opt => opt.Ignore());

        CreateMap<Crops, CropListDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ActiveOffersCount,
                opt => opt.MapFrom(src => src.Offers.Count())) // Simply count all offers
            .ForMember(dest => dest.MainAttributes,
                opt => opt.MapFrom(src => src.CropAttributes
                    .GroupBy(attr => new { attr.AttributeDefinition.CategoryId, CategoryName = attr.AttributeDefinition.Category.Name })
                    .Select(group => new CategoryWithAttributesDto
                    {
                        CategoryId = group.Key.CategoryId,
                        CategoryName = group.Key.CategoryName,
                        AttributeValues = group.ToDictionary(
                            attr => attr.AttributeDefinition.Name,
                            attr => attr.Value
                        )
                    })
                    .ToList()));
    }

    private static List<string> GetCategoryPath(CropCategories category)
    {
        var path = new List<string>();
        var current = category;

        while (current != null)
        {
            path.Insert(0, current.Name);
            current = current.ParentCategory;
        }

        return path;
    }
}
