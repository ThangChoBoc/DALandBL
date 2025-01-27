namespace ZelnyTrh.EF.BL.DTOs.UserDto;

public class UserPasswordChangeDto
{
    public required string Id { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}