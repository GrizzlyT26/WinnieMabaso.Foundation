using System.ComponentModel.DataAnnotations;

namespace WinnieMabaso.Application.DTOs.VolunteerApplications;

public class CreateVolunteerApplication
{
    [Required]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage =
            "Full name must be between 2 and 100 characters.")]
    public string FullName { get; set; } = string.Empty;


    [Required]
    [EmailAddress]
    [StringLength(
        150,
        ErrorMessage =
            "Email address cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;


    [Required]
    [Phone]
    [StringLength(
        20,
        ErrorMessage =
            "Mobile number cannot exceed 20 characters.")]
    public string MobileNumber { get; set; } = string.Empty;


    [Required]
    [StringLength(
        1000,
        MinimumLength = 2,
        ErrorMessage =
            "Skills must be between 2 and 1000 characters.")]
    public string Skills { get; set; } = string.Empty;


    [Required]
    [StringLength(
        500,
        MinimumLength = 2,
        ErrorMessage =
            "Availability must be between 2 and 500 characters.")]
    public string Availability { get; set; } = string.Empty;


    [Required]
    [StringLength(
        2000,
        MinimumLength = 10,
        ErrorMessage =
            "Motivation must be between 10 and 2000 characters.")]
    public string Motivation { get; set; } = string.Empty;
}