using Google.Cloud.Firestore;
using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Domain.Entities;

[FirestoreData]
public class VolunteerApplication
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string VolunteerUserId { get; set; } = string.Empty;

    [FirestoreProperty]
    public string FullName { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Email { get; set; } = string.Empty;

    [FirestoreProperty]
    public string MobileNumber { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Skills { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Availability { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Motivation { get; set; } = string.Empty;

    [FirestoreProperty]
    public VolunteerApplicationStatus Status { get; set; }

    [FirestoreProperty]
    public string? AdminNotes { get; set; }

    [FirestoreProperty]
    public string? ProcessedByUserId { get; set; }

    [FirestoreProperty]
    public DateTime? ProcessedAt { get; set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; set; }

    [FirestoreProperty]
    public DateTime UpdatedAt { get; set; }
}