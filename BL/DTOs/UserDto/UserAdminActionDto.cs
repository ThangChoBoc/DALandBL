namespace ZelnyTrh.EF.BL.DTOs.UserDto;

public class UserAdminActionDto
{
    public required string UserId { get; set; }
    public required string Action { get; set; } // e.g., "PromoteToModerator", "Suspend", "Activate"
    public string? Reason { get; set; }
}