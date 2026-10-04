using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Application.DTOs.AssistanceRequests;

public class AssistanceRequestResponse
{
    public string Id { get; set; } =
        string.Empty;


    public string Reference { get; set; } =
        string.Empty;


    public string BeneficiaryUserId { get; set; } =
        string.Empty;


    public string Category { get; set; } =
        string.Empty;


    public string Description { get; set; } =
        string.Empty;


    public int HouseholdSize { get; set; }


    public string Urgency { get; set; } =
        string.Empty;


    public string? ProgrammeId { get; set; }


    public AssistanceRequestStatus Status { get; set; }


    public string? DecisionReason { get; set; }


    public string? CapturedByUserId { get; set; }


    public DateTime CreatedAt { get; set; }


    public DateTime UpdatedAt { get; set; }
}