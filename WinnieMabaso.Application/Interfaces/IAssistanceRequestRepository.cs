using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.Interfaces;

public interface IAssistanceRequestRepository
{
    Task<AssistanceRequest?> GetByIdAsync(
        string id);

    Task<AssistanceRequest?> GetByReferenceAsync(
        string reference);

    Task<List<AssistanceRequest>>
        GetByBeneficiaryAsync(
            string beneficiaryUserId);

    Task<List<AssistanceRequest>>
        GetAllAsync();

    Task CreateAsync(
        AssistanceRequest request);

    Task UpdateAsync(
        AssistanceRequest request);
}