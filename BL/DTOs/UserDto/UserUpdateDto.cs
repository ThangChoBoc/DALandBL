namespace ZelnyTrh.EF.BL.DTOs.UserDto;

public class UserUpdateDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}