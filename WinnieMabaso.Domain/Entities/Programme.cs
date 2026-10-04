using Google.Cloud.Firestore;

namespace WinnieMabaso.Domain.Entities;

[FirestoreData]
public class Programme
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Name { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Description { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Category { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Location { get; set; } = string.Empty;

    [FirestoreProperty]
    public DateTime StartDate { get; set; }

    [FirestoreProperty]
    public DateTime? EndDate { get; set; }

    [FirestoreProperty]
    public int Capacity { get; set; }

    [FirestoreProperty]
    public bool IsActive { get; set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; set; }

    [FirestoreProperty]
    public DateTime UpdatedAt { get; set; }

    [FirestoreProperty]
    public string? CreatedByUserId { get; set; }

    [FirestoreProperty]
    public string? UpdatedByUserId { get; set; }
}