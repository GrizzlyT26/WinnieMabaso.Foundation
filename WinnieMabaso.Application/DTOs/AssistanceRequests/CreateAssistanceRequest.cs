using System.ComponentModel.DataAnnotations;

namespace WinnieMabaso.Application.DTOs.AssistanceRequests;

public class CreateAssistanceRequest
{
    [Required]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage =
            "Category must be between 2 and 100 characters.")]
    public string Category { get; set; } =
        string.Empty;


    [Required]
    [StringLength(
        2000,
        MinimumLength = 10,
        ErrorMessage =
            "Description must be between 10 and 2000 characters.")]
    public string Description { get; set; } =
        string.Empty;


    [Range(
        1,
        50,
        ErrorMessage =
            "Household size must be between 1 and 50.")]
    public int HouseholdSize { get; set; }


    [Required]
    [StringLength(
        30,
        MinimumLength = 2,
        ErrorMessage =
            "Urgency is required.")]
    public string Urgency { get; set; } =
        string.Empty;


    public string? ProgrammeId { get; set; }
}