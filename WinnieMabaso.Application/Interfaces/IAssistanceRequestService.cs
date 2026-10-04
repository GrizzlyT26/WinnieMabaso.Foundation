using WinnieMabaso.Application.DTOs.AssistanceRequests;

namespace WinnieMabaso.Application.Interfaces;

public interface IAssistanceRequestService
{
    Task<AssistanceRequestResponse> CreateAsync(
        string beneficiaryUserId,
        CreateAssistanceRequest request);

    Task<List<AssistanceRequestResponse>>
        GetMyRequestsAsync(
            string beneficiaryUserId);

    Task<AssistanceRequestResponse?>
        GetByIdAsync(
            string requestId,
            string beneficiaryUserId);

    Task<List<AssistanceRequestResponse>>
        GetAllAsync();

    Task UpdateStatusAsync(
        string requestId,
        string status,
        string decisionReason,
        string processedByUserId);
}