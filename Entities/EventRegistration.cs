using Google.Cloud.Firestore;
using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Domain.Entities;

[FirestoreData]
public class EventRegistration
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string EventId { get; set; } = string.Empty;

    [FirestoreProperty]
    public string UserId { get; set; } = string.Empty;

    [FirestoreProperty]
    public UserRole UserRole { get; set; }

    [FirestoreProperty]
    public EventRegistrationStatus Status { get; set; }

    [FirestoreProperty]
    public DateTime RegisteredAt { get; set; }

    [FirestoreProperty]
    public DateTime? CancelledAt { get; set; }

    [FirestoreProperty]
    public string? CancellationReason { get; set; }

    [FirestoreProperty]
    public DateTime UpdatedAt { get; set; }
}
