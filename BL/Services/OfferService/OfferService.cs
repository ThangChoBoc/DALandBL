using AutoMapper;
using Microsoft.Extensions.Logging;
using ZelnyTrh.EF.BL.DTOs.OfferDTO;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.UnitsOfWork;

namespace ZelnyTrh.EF.BL.Services.OfferService;

public class OfferService : IOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OfferService> _logger;

    public OfferService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OfferService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<OfferListDto>> GetAllOffersAsync()
    {
        try
        {
            // Fetch all offers
            var offers = await _unitOfWork.GetRepository<Offers>().GetAllAsync();

            // Convert to list to avoid multiple enumerations
            var offersList = offers.ToList();

            // Load related User and Crop data manually
            var userIds = offersList.Select(o => o.UserId).Distinct();
            var cropIds = offersList.Select(o => o.CropId).Distinct();

            // Fetch related data
            var users = await _unitOfWork.GetRepository<ApplicationUser>().GetByConditionAsync(u => userIds.Contains(u.Id));
            var crops = await _unitOfWork.GetRepository<Crops>().GetByConditionAsync(c => cropIds.Contains(c.Id));

            // Create dictionaries for quick lookup
            var userDictionary = users.ToDictionary(u => u.Id, u => u);
            var cropDictionary = crops.ToDictionary(c => c.Id, c => c);

            // Map Offers to OfferListDto and fill in related data
            var offerListDtos = offersList.Select(o =>
            {
                var dto = _mapper.Map<OfferListDto>(o);

                // Set FarmerName and CropName if available
                if (userDictionary.TryGetValue(o.UserId, out var user))
                {
                    dto.FarmerName = user.Name;
                }

                if (cropDictionary.TryGetValue(o.CropId, out var crop))
                {
                    dto.CropName = crop.Name;
                }

                return dto;
            });

            return offerListDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all offers");
            throw;
        }
    }

    public async Task<OfferReadDto> GetOfferByIdAsync(string id)
    {
        try
        {
            var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(id);
            if (offer == null)
                throw new KeyNotFoundException($"Offer with ID {id} not found.");

            return _mapper.Map<OfferReadDto>(offer);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving offer with ID {OfferId}", id);
            throw;
        }
    }
    public async Task<IEnumerable<OfferListDto>> GetOffersByUserAsync(string userId)
    {
        try
        {
            // Step 1: Get all offers for the specific user
            var offers = await _unitOfWork.GetRepository<Offers>().GetByConditionAsync(o => o.UserId == userId);
            var offersList = offers.ToList();

            // Step 2: Extract distinct User and Crop IDs from offers
            var cropIds = offersList.Select(o => o.CropId).Distinct().ToList();
            var userIds = offersList.Select(o => o.UserId).Distinct().ToList();

            // Step 3: Fetch related User and Crop data manually
            var users = await _unitOfWork.GetRepository<ApplicationUser>().GetByConditionAsync(u => userIds.Contains(u.Id));
            var crops = await _unitOfWork.GetRepository<Crops>().GetByConditionAsync(c => cropIds.Contains(c.Id));

            // Create dictionaries for quick lookup
            var userDictionary = users.ToDictionary(u => u.Id, u => u);
            var cropDictionary = crops.ToDictionary(c => c.Id, c => c);

            // Step 4: Map Offers to OfferListDto and populate related data
            var offerListDtos = offersList.Select(o =>
            {
                var dto = _mapper.Map<OfferListDto>(o);

                // Set FarmerName and CropName if available
                if (userDictionary.TryGetValue(o.UserId, out var user))
                {
                    dto.FarmerName = user.Name;
                }

                if (cropDictionary.TryGetValue(o.CropId, out var crop))
                {
                    dto.CropName = crop.Name;
                }

                return dto;
            }).ToList();

            return offerListDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving offers for user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<OfferListDto>> GetOffersByCategoryAsync(string categoryId)
    {
        try
        {
            var offers = await _unitOfWork.GetRepository<Offers>()
                .GetByConditionAsync(o => o.Crop.CategoryId == categoryId);

            return offers.Select(o => _mapper.Map<OfferListDto>(o));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving offers for category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<OfferListDto>> GetActiveOffersAsync()
    {
        try
        {
            var offers = await _unitOfWork.GetRepository<Offers>()
                .GetByConditionAsync(o => o.UnitsAvailable > 0);

            return offers.Select(o => _mapper.Map<OfferListDto>(o));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active offers");
            throw;
        }
    }

    public async Task<IEnumerable<OfferListDto>> SearchOffersAsync(OfferSearchDto searchDto)
    {
        try
        {
            var query = await _unitOfWork.GetRepository<Offers>().GetAllAsync();
            var offersQuery = query.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.CategoryId))
            {
                offersQuery = offersQuery.Where(o => o.Crop.CategoryId == searchDto.CategoryId);
            }

            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                var searchTermLower = searchDto.SearchTerm.ToLower();
                offersQuery = offersQuery.Where(o =>
                    o.Name.ToLower().Contains(searchTermLower) ||
                    o.Crop.Name.ToLower().Contains(searchTermLower));
            }

            if (searchDto.MinPrice.HasValue)
            {
                offersQuery = offersQuery.Where(o => o.Price >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                offersQuery = offersQuery.Where(o => o.Price <= searchDto.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(searchDto.Location))
            {
                offersQuery = offersQuery.Where(o =>
                    o.Origin.ToLower().Contains(searchDto.Location.ToLower()));
            }

            if (searchDto.OfferType.HasValue)
            {
                offersQuery = offersQuery.Where(o => o.OfferType == searchDto.OfferType.Value);
            }

            if (searchDto.AvailableOnly)
            {
                offersQuery = offersQuery.Where(o => o.UnitsAvailable > 0);
            }

            // Apply sorting
            offersQuery = ApplySorting(offersQuery, searchDto.SortBy, searchDto.SortDescending);

            var offers = offersQuery.ToList();
            return offers.Select(o => _mapper.Map<OfferListDto>(o));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching offers");
            throw;
        }
    }

    public async Task<OfferReadDto> CreateOfferAsync(OfferCreateDto offerCreateDto)
    {
        try
        {
            // Validate user exists
            var userExists = await _unitOfWork.GetRepository<ApplicationUser>()
                .ExistsAsync(offerCreateDto.UserId);
            if (!userExists)
                throw new KeyNotFoundException($"User with ID {offerCreateDto.UserId} not found.");

            // Validate crop exists
            var cropExists = await _unitOfWork.GetRepository<Crops>()
                .ExistsAsync(offerCreateDto.CropId);
            if (!cropExists)
                throw new KeyNotFoundException($"Crop with ID {offerCreateDto.CropId} not found.");

            // Check for existing offer
            var existingOffers = await _unitOfWork.GetRepository<Offers>()
                .GetByConditionAsync(o =>
                    o.UserId == offerCreateDto.UserId &&
                    o.CropId == offerCreateDto.CropId);

            if (existingOffers.Any())
                throw new InvalidOperationException("An offer for this crop by the user already exists.");

            // Create the offer
            var offer = _mapper.Map<Offers>(offerCreateDto);
            offer.Id = Guid.NewGuid().ToString();

            await _unitOfWork.GetRepository<Offers>().InsertAsync(offer);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<OfferReadDto>(offer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating offer");
            throw;
        }
    }

    public async Task<OfferReadDto> UpdateOfferAsync(OfferUpdateDto offerUpdateDto, string currentUserId)
    {
        try
        {
            var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(offerUpdateDto.Id);
            if (offer == null)
                throw new KeyNotFoundException($"Offer with ID {offerUpdateDto.Id} not found.");

            // Validate ownership
            if (offer.UserId != currentUserId)
                throw new UnauthorizedAccessException("You do not have permission to update this offer.");

            // Validate if the offer can be updated
            if (offer.OrderOffers.Any())
            {
                throw new InvalidOperationException("Cannot update offer with existing orders.");
            }

            _mapper.Map(offerUpdateDto, offer);
            await _unitOfWork.GetRepository<Offers>().UpdateAsync(offer);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<OfferReadDto>(offer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating offer {OfferId}", offerUpdateDto.Id);
            throw;
        }
    }

    public async Task DeleteOfferAsync(string id)
    {
        try
        {
            var offer = await _unitOfWork.GetRepository<Offers>().GetByIdAsync(id);
            if (offer == null)
                throw new KeyNotFoundException($"Offer with ID {id} not found.");

            // Check if offer can be deleted
            if (offer.OrderOffers.Any())
            {
                throw new InvalidOperationException("Cannot delete offer with existing orders.");
            }

            await _unitOfWork.GetRepository<Offers>().DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting offer {OfferId}", id);
            throw;
        }
    }

    private static IQueryable<Offers> ApplySorting(
        IQueryable<Offers> query,
        string? sortBy,
        bool sortDescending)
    {
        query = sortBy?.ToLower() switch
        {
            "price" => sortDescending
                ? query.OrderByDescending(o => o.Price)
                : query.OrderBy(o => o.Price),
            "name" => sortDescending
                ? query.OrderByDescending(o => o.Name)
                : query.OrderBy(o => o.Name),
            _ => query.OrderByDescending(o => o.Id)
        };

        return query;
    }
}