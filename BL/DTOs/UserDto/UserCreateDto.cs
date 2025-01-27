namespace ZelnyTrh.EF.BL.DTOs.UserDto;

public class UserCreateDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? PhoneNumber { get; set; }
}