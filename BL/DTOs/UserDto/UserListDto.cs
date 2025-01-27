namespace ZelnyTrh.EF.BL.DTOs.UserDto;

public class UserListDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = [];
    public DateTime RegisteredAt { get; set; }
}