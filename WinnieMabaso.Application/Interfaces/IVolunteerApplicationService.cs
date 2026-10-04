using WinnieMabaso.Application.DTOs.VolunteerApplications;

namespace WinnieMabaso.Application.Interfaces;

public interface IVolunteerApplicationService
{
    Task<VolunteerApplicationResponse> CreateAsync(
        string volunteerUserId,
        CreateVolunteerApplication request);

    Task<List<VolunteerApplicationResponse>>
        GetMyApplicationsAsync(
            string volunteerUserId);

    Task<VolunteerApplicationResponse?>
        GetByIdAsync(
            string applicationId,
            string volunteerUserId);

    Task<List<VolunteerApplicationResponse>>
        GetAllAsync();

    Task UpdateStatusAsync(
        string applicationId,
        string status,
        string adminNotes,
        string processedByUserId);
}