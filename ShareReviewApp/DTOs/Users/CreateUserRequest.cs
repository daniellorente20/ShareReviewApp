using System.ComponentModel.DataAnnotations;

namespace ShareReviewApp.DTOs.Users;

public class CreateUserRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
