using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Application.DTOs.VolunteerApplications;

public class VolunteerApplicationResponse
{
    public string Id { get; set; } = string.Empty;

    public string VolunteerUserId { get; set; } =
        string.Empty;

    public string FullName { get; set; } =
        string.Empty;

    public string Email { get; set; } =
        string.Empty;

    public string MobileNumber { get; set; } =
        string.Empty;

    public string Skills { get; set; } =
        string.Empty;

    public string Availability { get; set; } =
        string.Empty;

    public string Motivation { get; set; } =
        string.Empty;

    public VolunteerApplicationStatus Status { get; set; }

    public string? AdminNotes { get; set; }

    public string? ProcessedByUserId { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}