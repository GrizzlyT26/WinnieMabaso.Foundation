namespace WinnieMabaso.Application.DTOs.Events;

public class EventResponse
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public string Location { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public DateTime RegistrationDeadline { get; set; }

    public List<string> EligibleRoles { get; set; } = new();

    public bool Published { get; set; }

    public bool Cancelled { get; set; }

    public int RegisteredCount { get; set; }

    public bool IsFull { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? CreatedByUserId { get; set; }

    public string? UpdatedByUserId { get; set; }
}