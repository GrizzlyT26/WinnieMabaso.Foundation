using System.ComponentModel.DataAnnotations;

namespace WinnieMabaso.Application.DTOs.Users;

public class UpdateProfileRequest
{
    [Required]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Full name must be between 2 and 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(
        20,
        ErrorMessage = "Mobile number cannot exceed 20 characters.")]
    public string MobileNumber { get; set; } = string.Empty;
}