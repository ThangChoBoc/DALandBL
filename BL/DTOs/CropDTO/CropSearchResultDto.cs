namespace ZelnyTrh.EF.BL.DTOs.CropDTO;

public class CropSearchResultDto
{
    public List<CropListDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageCount { get; set; }
    public Dictionary<string, List<string>> AvailableAttributeValues { get; set; } = [];
}