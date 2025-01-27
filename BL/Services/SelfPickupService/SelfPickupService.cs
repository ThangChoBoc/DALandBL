using AutoMapper;
using ZelnyTrh.EF.BL.DTOs.SelfPickupDto;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.SelfPickupService;

public class SelfPickupService(IUnitOfWork unitOfWork, IMapper mapper) : ISelfPickupService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<SelfPickupReadDto> CreateSelfPickupAsync(SelfPickupCreateDto dto)
    {
        if (dto.Starting >= dto.Ending)
            throw new InvalidOperationException("End time must be after start time");

        var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(dto.OfferId);
        if (offer == null)
            throw new KeyNotFoundException($"Offer with ID {dto.OfferId} not found");

        var selfPickup = new SelfPickups
        {
            Id = Guid.NewGuid().ToString(),
            Location = dto.Location,
            Starting = dto.Starting,
            Ending = dto.Ending,
            OfferId = dto.OfferId,
            Offer = offer,
            Registrations = new List<SelfPickupRegistrations>()
        };

        await _unitOfWork.GetRepository<SelfPickups>().InsertAsync(selfPickup);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<SelfPickupReadDto>(selfPickup);
    }

    public async Task<IEnumerable<SelfPickupReadDto>> GetAvailableSelfPickupsAsync()
    {
        var currentTime = DateTime.UtcNow;
        var selfPickupsRepo = _unitOfWork.GetRepository<SelfPickups>();

        var selfPickups = await selfPickupsRepo.GetByConditionAsync(sp => sp.Ending > currentTime);

        // Map and populate additional data
        var result = new List<SelfPickupReadDto>();
        foreach (var selfPickup in selfPickups)
        {
            var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(selfPickup.OfferId);
            var farmer = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(offer.UserId);
            var registrations = await _unitOfWork.GetRepository<SelfPickupRegistrations>()
                .GetByConditionAsync(r => r.SelfPickupId == selfPickup.Id);

            result.Add(new SelfPickupReadDto
            {
                Id = selfPickup.Id,
                Location = selfPickup.Location,
                Starting = selfPickup.Starting,
                Ending = selfPickup.Ending,
                OfferId = selfPickup.OfferId,
                OfferName = offer.Name,
                FarmerId = offer.UserId,
                FarmerName = farmer.Name,
                RegisteredUsersCount = registrations.Count()
            });
        }

        return result.OrderBy(sp => sp.Starting);
    }

    public async Task<IEnumerable<SelfPickupReadDto>> GetUserRegisteredSelfPickupsAsync(string userId)
    {
        var registrations = await _unitOfWork.GetRepository<SelfPickupRegistrations>()
            .GetByConditionAsync(r => r.UserId == userId);

        var result = new List<SelfPickupReadDto>();
        foreach (var registration in registrations)
        {
            var selfPickup = await _unitOfWork.GetRepository<SelfPickups>().GetByIdAsync(registration.SelfPickupId);
            var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(selfPickup.OfferId);
            var farmer = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(offer.UserId);
            var allRegistrations = await _unitOfWork.GetRepository<SelfPickupRegistrations>()
                .GetByConditionAsync(r => r.SelfPickupId == selfPickup.Id);

            result.Add(new SelfPickupReadDto
            {
                Id = selfPickup.Id,
                Location = selfPickup.Location,
                Starting = selfPickup.Starting,
                Ending = selfPickup.Ending,
                OfferId = selfPickup.OfferId,
                OfferName = offer.Name,
                FarmerId = offer.UserId,
                FarmerName = farmer.Name,
                RegisteredUsersCount = allRegistrations.Count()
            });
        }

        return result.OrderBy(sp => sp.Starting);
    }

    public async Task RegisterForSelfPickupAsync(string selfPickupId, string userId)
    {
        var selfPickup = await _unitOfWork.GetRepository<SelfPickups>().GetByIdAsync(selfPickupId);
        if (selfPickup == null)
            throw new KeyNotFoundException($"SelfPickup with ID {selfPickupId} not found");

        if (DateTime.UtcNow >= selfPickup.Ending)
            throw new InvalidOperationException("Cannot register for a self-pickup that has already ended");

        var existingRegistrations = await _unitOfWork.GetRepository<SelfPickupRegistrations>()
            .GetByConditionAsync(r => r.SelfPickupId == selfPickupId && r.UserId == userId);

        if (existingRegistrations.Any())
            throw new InvalidOperationException("User is already registered for this self-pickup");

        var user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var registration = new SelfPickupRegistrations
        {
            Id = Guid.NewGuid().ToString(),
            SelfPickupId = selfPickupId,
            SelfPickup = selfPickup,  // Set the required navigation property
            UserId = userId,
            User = user  // Set the required navigation property
        };

        await _unitOfWork.GetRepository<SelfPickupRegistrations>().InsertAsync(registration);
        await _unitOfWork.CommitAsync();
    }
    public async Task UnregisterFromSelfPickupAsync(string selfPickupId, string userId)
    {
        var registrationsRepo = _unitOfWork.GetRepository<SelfPickupRegistrations>();
        var registrations = await registrationsRepo
            .GetByConditionAsync(r => r.SelfPickupId == selfPickupId && r.UserId == userId);

        var registration = registrations.FirstOrDefault();
        if (registration == null)
            throw new KeyNotFoundException($"Registration not found for user {userId} and self-pickup {selfPickupId}");

        var selfPickup = await _unitOfWork.GetRepository<SelfPickups>().GetByIdAsync(selfPickupId);
        if (DateTime.UtcNow >= selfPickup.Ending)
            throw new InvalidOperationException("Cannot unregister from a self-pickup that has already ended");

        await registrationsRepo.DeleteAsync(registration.Id);
        await _unitOfWork.CommitAsync();
    }
}
