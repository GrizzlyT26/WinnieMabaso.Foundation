using System.ComponentModel.DataAnnotations;

namespace WinnieMabaso.Application.DTOs.Programmes;

public class CreateProgramme
{
    [Required]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage =
            "Programme name must be between 2 and 150 characters.")]
    public string Name { get; set; } = string.Empty;


    [Required]
    [StringLength(
        3000,
        MinimumLength = 10,
        ErrorMessage =
            "Description must be between 10 and 3000 characters.")]
    public string Description { get; set; } = string.Empty;


    [Required]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage =
            "Category must be between 2 and 100 characters.")]
    public string Category { get; set; } = string.Empty;


    [Required]
    [StringLength(
        250,
        MinimumLength = 2,
        ErrorMessage =
            "Location must be between 2 and 250 characters.")]
    public string Location { get; set; } = string.Empty;


    [Required]
    public DateTime StartDate { get; set; }


    public DateTime? EndDate { get; set; }


    [Range(
        1,
        100000,
        ErrorMessage =
            "Capacity must be between 1 and 100000.")]
    public int Capacity { get; set; }
}