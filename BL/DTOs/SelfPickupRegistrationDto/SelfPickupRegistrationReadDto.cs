namespace ZelnyTrh.EF.BL.DTOs.SelfPickupRegistrationDto;

public class SelfPickupRegistrationReadDto
{
    public required string Id { get; set; }
    public required string SelfPickupId { get; set; }
    public required string UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}