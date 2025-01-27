namespace ZelnyTrh.EF.BL.DTOs.ReviewsDto;

public class ReviewUpdateDto
{
    public int Rating { get; set; }
    public string ReviewDescription { get; set; } = string.Empty;
}