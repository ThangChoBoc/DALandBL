using ZelnyTrh.EF.BL.DTOs.SelfPickupRegistrationDto;

namespace ZelnyTrh.EF.BL.Services.SelfPickupRegistrationService;

public interface ISelfPickupRegistrationService
{
    Task<SelfPickupRegistrationReadDto> CreateRegistrationAsync(SelfPickupRegistrationCreateDto dto);
    Task<IEnumerable<SelfPickupRegistrationReadDto>> GetRegistrationsBySelfPickupAsync(string selfPickupId);
    Task<IEnumerable<SelfPickupRegistrationReadDto>> GetRegistrationsByUserAsync(string userId);
    Task DeleteRegistrationAsync(string registrationId);
}