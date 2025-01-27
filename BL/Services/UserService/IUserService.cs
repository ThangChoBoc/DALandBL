using ZelnyTrh.EF.BL.DTOs.UserDto;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.BL.Services.UserService;

public interface IUserService
{
    Task<bool> PromoteToModeratorAsync(string userId, string adminId);
    Task<bool> DemoteToUser(string userId, string adminId);
    Task<bool> GetOffersFromThisUser(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string role);
    Task<IEnumerable<UserListDto>> GetAllUsersAsync();
    Task<IEnumerable<UserListDto>> GetAllFarmersWithOffersAsync();
}