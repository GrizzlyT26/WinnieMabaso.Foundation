using Google.Cloud.Firestore;

namespace WinnieMabaso.Domain.Entities;

[FirestoreData]
public class VolunteerDocument
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string VolunteerUserId { get; set; } = string.Empty;

    [FirestoreProperty]
    public string VolunteerApplicationId { get; set; } = string.Empty;

    [FirestoreProperty]
    public string OriginalFileName { get; set; } = string.Empty;

    [FirestoreProperty]
    public string StoredFileName { get; set; } = string.Empty;

    [FirestoreProperty]
    public string ContentType { get; set; } = string.Empty;

    [FirestoreProperty]
    public long FileSize { get; set; }

    [FirestoreProperty]
    public string StoragePath { get; set; } = string.Empty;

    [FirestoreProperty]
    public string? DownloadUrl { get; set; }

    [FirestoreProperty]
    public string DocumentType { get; set; } = string.Empty;

    [FirestoreProperty]
    public DateTime UploadedAt { get; set; }

    [FirestoreProperty]
    public string? UploadedByUserId { get; set; }
}