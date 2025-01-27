using ZelnyTrh.EF.BL.DTOs.SelfPickupDto;

namespace ZelnyTrh.EF.BL.Services.SelfPickupService;

public interface ISelfPickupService
{
    Task<SelfPickupReadDto> CreateSelfPickupAsync(SelfPickupCreateDto dto);
    Task<IEnumerable<SelfPickupReadDto>> GetAvailableSelfPickupsAsync();
    Task<IEnumerable<SelfPickupReadDto>> GetUserRegisteredSelfPickupsAsync(string userId);
    Task RegisterForSelfPickupAsync(string selfPickupId, string userId);
    Task UnregisterFromSelfPickupAsync(string selfPickupId, string userId);
}