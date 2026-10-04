using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Application.DTOs.EventRegistrations;

public class EventRegistrationResponse
{
    public string Id { get; set; } = string.Empty;

    public string EventId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public UserRole UserRole { get; set; }

    public EventRegistrationStatus Status { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime UpdatedAt { get; set; }
}