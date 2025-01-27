using ZelnyTrh.EF.BL.DTOs.CropCategoriesDto;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.Services.CropCategoriesService;

public interface ICropCategoryService
{
    Task<CropCategoryReadDto> CreateCategoryAsync(CropCategoryCreateDto dto);
    Task<CropCategoryReadDto> GetCategoryByIdAsync(string id);
    Task<IEnumerable<CropCategoryReadDto>> GetRootCategoriesAsync();
    Task<CropCategoryReadDto> UpdateCategoryStatusAsync(string id, CropCategoryStatus newStatus, string moderatorId);
    Task DeleteCategoryAsync(string id);
    Task<IEnumerable<CropCategoryReadDto>> GetCategoryPathAsync(string categoryId);
    Task<bool> ValidateCategoryHierarchyAsync(string parentId, string? childId = null);
    Task<CropCategoryReadDto> UpdateCategoryAsync(CropCategoryUpdateDto dto);
    Task<int> DeleteRejectedCategoriesAsync(string moderatorId);
}