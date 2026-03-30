using System.ComponentModel.DataAnnotations;

namespace ShareReviewApp.DTOs.Reviews;

public class CreateReviewRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string Comment { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int Rating { get; set; }
}
