using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using ZelnyTrh.EF.BL.DTOs.UserDto;
using ZelnyTrh.EF.DAL;
using ZelnyTrh.EF.DAL.Entities;
using ZelnyTrh.EF.DAL.Repositories;

namespace ZelnyTrh.EF.BL.Services.UserService;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IRepository<Offers> _offerRepository;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IMapper mapper,
        IRepository<ApplicationUser> userRepository,
        IRepository<Offers> offerRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _mapper = mapper;
        _userRepository = userRepository;
        _offerRepository = offerRepository;
    }

    public async Task<IEnumerable<UserListDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var userList = await users.ToListAsync();

        // Fetch all user roles
        var userRoles = await _context.UserRoles.ToListAsync();

        // Fetch all roles
        var roles = await _context.Roles.ToListAsync();

        // Create a dictionary for quick role name lookup
        var roleDict = roles.ToDictionary(r => r.Id, r => r.Name);

        // Group user roles by user ID
        var userRolesDict = userRoles.GroupBy(ur => ur.UserId)
            .ToDictionary(grp => grp.Key, grp => grp.Select(ur => roleDict[ur.RoleId]).ToList());

        // Map users to UserListDto
        var userListDtos = userList.Select(u => new UserListDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email!,
            PhoneNumber = u.PhoneNumber,
            IsEmailConfirmed = u.EmailConfirmed,
            Roles = userRolesDict.ContainsKey(u.Id) ? userRolesDict[u.Id] : new List<string>(),
            // RegisteredAt = u.RegisteredAt // Include if available
        }).ToList();

        return userListDtos;
    }


    public async Task<bool> PromoteToModeratorAsync(string userId, string adminId)
    {
        var adminUser = await _userManager.FindByIdAsync(adminId);
        if (adminUser == null || !await _userManager.IsInRoleAsync(adminUser, "Administrator"))
        {
            throw new UnauthorizedAccessException("Only administrators can promote users to moderators");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        // Add user to Moderator role and claims in a single operation
        var addToRoleResult = await _userManager.AddToRoleAsync(user, "Moderator");
        if (!addToRoleResult.Succeeded) return false;

        var moderatorClaims = new List<Claim>
        {
            new Claim("ModeratorSince", DateTime.UtcNow.ToString()),
            new Claim("PromotedBy", adminId)
        };

        // Add claims in bulk
        var addClaimsResult = await _userManager.AddClaimsAsync(user, moderatorClaims);
        return addClaimsResult.Succeeded;
    }

    public async Task<bool> DemoteToUser(string userId, string adminId)
    {
        var adminUser = await _userManager.FindByIdAsync(adminId);
        if (adminUser == null || !await _userManager.IsInRoleAsync(adminUser, "Administrator"))
        {
            throw new UnauthorizedAccessException("Only administrators can demote moderators");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // Remove from Moderator role
        var removeFromRoleResult = await _userManager.RemoveFromRoleAsync(user, "Moderator");
        if (!removeFromRoleResult.Succeeded)
        {
            return false;
        }

        // Remove moderator claims
        var claims = await _userManager.GetClaimsAsync(user);
        var moderatorClaims = claims.Where(c =>
            c.Type == "ModeratorSince" ||
            c.Type == "PromotedBy");

        foreach (var claim in moderatorClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        return true;
    }

    public async Task<bool> GetOffersFromThisUser(string userId)
    {
        // Optimize: Use caching for repeated calls if offers do not change frequently
        return await _context.Offers
            .AsNoTracking() // Improves performance if data does not need to be tracked
            .AnyAsync(o => o.UserId == userId);
    }


    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return await _userManager.IsInRoleAsync(user, role);
    }


    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        // Combine user existence check and role retrieval
        var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Enumerable.Empty<string>();
            }
            return await _userManager.GetRolesAsync(user);
    }
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string role)
    {
        return await _userManager.GetUsersInRoleAsync(role);
    }
    public async Task<IEnumerable<UserListDto>> GetAllFarmersWithOffersAsync()
    {
        // Combine distinct UserIds and user retrieval into a single query
        var farmers = await _context.Users
            .Where(u => _context.Offers.Any(o => o.UserId == u.Id)) // Inline subquery
            .Select(u => new UserListDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsEmailConfirmed = u.EmailConfirmed
            })
            .ToListAsync();

        return farmers;
    }
}
