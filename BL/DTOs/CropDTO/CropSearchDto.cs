namespace ZelnyTrh.EF.BL.DTOs.CropDTO;

public class CropSearchDto
{
    public string? SearchTerm { get; set; }
    public string? CategoryId { get; set; }
    public Dictionary<string, string> AttributeFilters { get; set; } = [];
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}