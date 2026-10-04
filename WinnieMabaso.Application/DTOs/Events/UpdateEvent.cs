using System.ComponentModel.DataAnnotations;

namespace WinnieMabaso.Application.DTOs.Events;

public class UpdateEvent
{
    [Required]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage = "Event title must be between 2 and 150 characters.")]
    public string Title { get; set; } = string.Empty;


    [Required]
    [StringLength(
        3000,
        MinimumLength = 10,
        ErrorMessage = "Event description must be between 10 and 3000 characters.")]
    public string Description { get; set; } = string.Empty;


    [Required]
    public DateTime StartsAt { get; set; }


    public DateTime? EndsAt { get; set; }


    [Required]
    [StringLength(
        250,
        MinimumLength = 2,
        ErrorMessage = "Location must be between 2 and 250 characters.")]
    public string Location { get; set; } = string.Empty;


    [Range(
        1,
        100000,
        ErrorMessage = "Capacity must be between 1 and 100000.")]
    public int Capacity { get; set; }


    [Required]
    public DateTime RegistrationDeadline { get; set; }


    [MinLength(
        1,
        ErrorMessage = "At least one eligible role must be specified.")]
    public List<string> EligibleRoles { get; set; } = new();
}