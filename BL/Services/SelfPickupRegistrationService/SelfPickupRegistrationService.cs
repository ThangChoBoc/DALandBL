using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.SelfPickupRegistrationDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.SelfPickupRegistrationService;

public class SelfPickupRegistrationService : ISelfPickupRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SelfPickupRegistrationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SelfPickupRegistrationReadDto> CreateRegistrationAsync(SelfPickupRegistrationCreateDto dto)
    {
        // Validate self-pickup exists and is not in the past
        var selfPickup = await _unitOfWork.GetRepository<SelfPickups>().GetByIdAsync(dto.SelfPickupId);
        if (selfPickup == null)
            throw new KeyNotFoundException($"SelfPickup with ID {dto.SelfPickupId} not found");

        if (DateTime.UtcNow >= selfPickup.Starting)
            throw new InvalidOperationException("Cannot register for a self-pickup that has already started");

        // Validate user exists
        var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(dto.UserId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {dto.UserId} not found");

        // Check if user is already registered
        var existingRegistrations = await _unitOfWork.GetRepository<SelfPickupRegistrations>()
            .GetByConditionAsync(r => r.SelfPickupId == dto.SelfPickupId && r.UserId == dto.UserId);

        if (existingRegistrations.Any())
            throw new InvalidOperationException("User is already registered for this self-pickup");

        // Create registration
        var registration = new SelfPickupRegistrations
        {
            Id = Guid.NewGuid().ToString(),
            SelfPickupId = dto.SelfPickupId,
            SelfPickup = selfPickup,
            UserId = dto.UserId,
            User = user
        };

        await _unitOfWork.GetRepository<SelfPickupRegistrations>().InsertAsync(registration);
        await _unitOfWork.CommitAsync();

        return new SelfPickupRegistrationReadDto
        {
            Id = registration.Id,
            SelfPickupId = registration.SelfPickupId,
            UserId = registration.UserId,
            UserName = user.Name,
            RegisteredAt = DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<SelfPickupRegistrationReadDto>> GetRegistrationsBySelfPickupAsync(string selfPickupId)
    {
        var registrations = await _unitOfWork.GetRepository<SelfPickupRegistrations>()
            .GetByConditionAsync(r => r.SelfPickupId == selfPickupId);

        var result = new List<SelfPickupRegistrationReadDto>();
        foreach (var registration in registrations)
        {
            var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(registration.UserId);
            result.Add(new SelfPickupRegistrationReadDto
            {
                Id = registration.Id,
                SelfPickupId = registration.SelfPickupId,
                UserId = registration.UserId,
                UserName = user.Name,
                RegisteredAt = DateTime.UtcNow // Note: You might want to add CreatedAt to your entity
            });
        }

        return result;
    }

    public async Task<IEnumerable<SelfPickupRegistrationReadDto>> GetRegistrationsByUserAsync(string userId)
    {
        var registrations = await _unitOfWork.GetRepository<SelfPickupRegistrations>()
            .GetByConditionAsync(r => r.UserId == userId);

        var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        return registrations.Select(registration => new SelfPickupRegistrationReadDto
        {
            Id = registration.Id,
            SelfPickupId = registration.SelfPickupId,
            UserId = registration.UserId,
            UserName = user.Name,
            RegisteredAt = DateTime.UtcNow // Note: You might want to add CreatedAt to your entity
        });
    }

    public async Task DeleteRegistrationAsync(string registrationId)
    {
        var registrationRepo = _unitOfWork.GetRepository<SelfPickupRegistrations>();

        // Check if registration exists
        if (!await registrationRepo.ExistsAsync(registrationId))
            throw new KeyNotFoundException($"Registration with ID {registrationId} not found");

        // Get registration to check self-pickup start time
        var registration = await registrationRepo.GetByIdAsync(registrationId);
        var selfPickup = await _unitOfWork.GetRepository<SelfPickups>().GetByIdAsync(registration.SelfPickupId);

        if (DateTime.UtcNow >= selfPickup.Starting)
            throw new InvalidOperationException("Cannot delete registration for a self-pickup that has already started");

        await registrationRepo.DeleteAsync(registrationId);
        await _unitOfWork.CommitAsync();
    }
}