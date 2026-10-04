using Google.Cloud.Firestore;
using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Domain.Entities;

[FirestoreData]
public class AssistanceRequest
{
    [FirestoreDocumentId]
    public string Id { get; set; } =
        string.Empty;


    [FirestoreProperty]
    public string Reference { get; set; } =
        string.Empty;


    [FirestoreProperty]
    public string BeneficiaryUserId { get; set; } =
        string.Empty;


    [FirestoreProperty]
    public string Category { get; set; } =
        string.Empty;


    [FirestoreProperty]
    public string Description { get; set; } =
        string.Empty;


    [FirestoreProperty]
    public int HouseholdSize { get; set; }


    [FirestoreProperty]
    public string Urgency { get; set; } =
        string.Empty;


    [FirestoreProperty]
    public string? ProgrammeId { get; set; }


    [FirestoreProperty]
    public AssistanceRequestStatus Status { get; set; }


    [FirestoreProperty]
    public string? DecisionReason { get; set; }


    [FirestoreProperty]
    public string? CapturedByUserId { get; set; }


    [FirestoreProperty]
    public DateTime CreatedAt { get; set; }


    [FirestoreProperty]
    public DateTime UpdatedAt { get; set; }
}