using ZelnyTrh.EF.BL.DTOs.AttributesDto;

namespace ZelnyTrh.EF.BL.Services.AttributeService;

public interface ICropAttributeService
{
    Task<CropAttributeReadDto> CreateCropAttributeAsync(CropAttributeCreateDto dto);
    Task<CropAttributeReadDto> GetCropAttributeByIdAsync(string id);
    Task<IEnumerable<CropAttributeReadDto>> GetAllCropAttributesAsync();
    Task<CropAttributeReadDto> UpdateCropAttributeAsync(CropAttributeUpdateDto dto);
    Task DeleteCropAttributeAsync(string id);
}