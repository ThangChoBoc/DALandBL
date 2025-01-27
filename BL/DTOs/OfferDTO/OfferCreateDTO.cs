using System.ComponentModel.DataAnnotations;
using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.OfferDTO;

public class OfferCreateDto
{
    public required string Name { get; set; }
    public required double Price { get; set; }
    public required string Currency { get; set; }
    public required decimal Amount { get; set; }
    public required int UnitsAvailable { get; set; }
    public required string Origin { get; set; }
    public required OfferType OfferType { get; set; }
    public required string UserId { get; set; }
    public required string CropId { get; set; }
}
