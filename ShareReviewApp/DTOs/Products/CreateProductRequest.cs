using System.ComponentModel.DataAnnotations;

namespace ShareReviewApp.DTOs.Products;

public class CreateProductRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;
}
