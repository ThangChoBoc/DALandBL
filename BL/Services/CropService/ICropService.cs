using ZelnyTrh.EF.BL.DTOs.CropDTO;
using ZelnyTrh.EF.BL.DTOs.OfferDTO;

namespace ZelnyTrh.EF.BL.Services.CropService;

public interface ICropService
{
    Task<CropReadDto> CreateCropAsync(CropCreateDto dto);
    Task<IEnumerable<CropListDto>> GetAllCropIdAsync();
    Task<CropReadDto> GetCropByIdAsync(string id);
    Task<CropSearchResultDto> SearchCropsAsync(CropSearchDto searchDto);
    Task<CropReadDto> UpdateCropAsync(CropUpdateDto dto);
    Task DeleteCropAsync(string id);
}