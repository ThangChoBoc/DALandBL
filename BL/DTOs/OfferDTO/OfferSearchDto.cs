using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.OfferDTO;

public class OfferSearchDto
{
    public string? CategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public string? Location { get; set; }
    public OfferType? OfferType { get; set; }
    public bool AvailableOnly { get; set; } = true;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}