using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.AttributesDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Repositories;

namespace ZelnyTrh.EF.BL.Services.AttributeService;

public class CropAttributeService(IRepository<CropAttributes> cropAttributeRepository, IMapper mapper)
    : ICropAttributeService
{
    public async Task<CropAttributeReadDto> CreateCropAttributeAsync(CropAttributeCreateDto dto)
    {
        var entity = mapper.Map<CropAttributes>(dto);
        await cropAttributeRepository.InsertAsync(entity);
        return mapper.Map<CropAttributeReadDto>(entity);
    }

    public async Task<CropAttributeReadDto> GetCropAttributeByIdAsync(string id)
    {
        var entity = await cropAttributeRepository.GetByIdAsync(id);
        return mapper.Map<CropAttributeReadDto>(entity);
    }

    public async Task<IEnumerable<CropAttributeReadDto>> GetAllCropAttributesAsync()
    {
        var entities = await cropAttributeRepository.GetAllAsync();
        return mapper.Map<IEnumerable<CropAttributeReadDto>>(entities);
    }

    public async Task<CropAttributeReadDto> UpdateCropAttributeAsync(CropAttributeUpdateDto dto)
    {
        var entity = await cropAttributeRepository.GetByIdAsync(dto.Id);
        mapper.Map(dto, entity);
        await cropAttributeRepository.UpdateAsync(entity);
        return mapper.Map<CropAttributeReadDto>(entity);
    }

    public async Task DeleteCropAttributeAsync(string id)
    {
        await cropAttributeRepository.DeleteAsync(id);
    }
}