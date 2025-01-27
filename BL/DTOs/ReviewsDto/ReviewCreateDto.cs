using System.ComponentModel.DataAnnotations;

namespace ZelnyTrh.EF.BL.DTOs.ReviewsDto;

public class ReviewCreateDto
{
    public required string OfferId { get; set; }

    [Range(1, 5)]
    public required int Rating { get; set; }

    [StringLength(1000)]
    public required string ReviewDescription { get; set; }
}