using System.ComponentModel.DataAnnotations;

namespace WinnieMabaso.Application.DTOs.EventRegistrations;

public class CreateEventRegistration
{
    [Required]
    public string EventId { get; set; } = string.Empty;
}