using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.Interfaces;

public interface IVolunteerDocumentRepository
{
    Task<VolunteerDocument?> GetByIdAsync(
        string id);

    Task<List<VolunteerDocument>>
        GetByVolunteerUserIdAsync(
            string volunteerUserId);

    Task<List<VolunteerDocument>>
        GetByApplicationIdAsync(
            string volunteerApplicationId);

    Task CreateAsync(
        VolunteerDocument document);

    Task DeleteAsync(
        string id);
}