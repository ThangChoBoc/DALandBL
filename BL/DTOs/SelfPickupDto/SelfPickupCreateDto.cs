using System.ComponentModel.DataAnnotations;

namespace ZelnyTrh.EF.BL.DTOs.SelfPickupDto;

public class SelfPickupCreateDto
{
    public required string Location { get; set; }
    [DataType(DataType.DateTime)]
    public required DateTime Starting { get; set; }
    [DataType(DataType.DateTime)]
    public required DateTime Ending { get; set; }
    public required string OfferId { get; set; }
}