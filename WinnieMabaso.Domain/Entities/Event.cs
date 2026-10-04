using Google.Cloud.Firestore;

namespace WinnieMabaso.Domain.Entities;

[FirestoreData]
public class Event
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;


    [FirestoreProperty]
    public string Title { get; set; } = string.Empty;


    [FirestoreProperty]
    public string Description { get; set; } = string.Empty;


    [FirestoreProperty]
    public DateTime StartsAt { get; set; }


    [FirestoreProperty]
    public DateTime? EndsAt { get; set; }


    [FirestoreProperty]
    public string Location { get; set; } = string.Empty;


    [FirestoreProperty]
    public int Capacity { get; set; }


    [FirestoreProperty]
    public DateTime RegistrationDeadline { get; set; }


    [FirestoreProperty]
    public List<string> EligibleRoles { get; set; } = new();


    [FirestoreProperty]
    public bool Published { get; set; }


    [FirestoreProperty]
    public bool Cancelled { get; set; }


    [FirestoreProperty]
    public DateTime CreatedAt { get; set; }


    [FirestoreProperty]
    public DateTime UpdatedAt { get; set; }


    [FirestoreProperty]
    public string? CreatedByUserId { get; set; }


    [FirestoreProperty]
    public string? UpdatedByUserId { get; set; }
}