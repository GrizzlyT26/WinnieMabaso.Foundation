using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.Interfaces;

public interface IVolunteerApplicationRepository
{
    Task<VolunteerApplication?> GetByIdAsync(
        string id);

    Task<List<VolunteerApplication>>
        GetByVolunteerUserIdAsync(
            string volunteerUserId);

    Task<List<VolunteerApplication>>
        GetAllAsync();

    Task CreateAsync(
        VolunteerApplication application);

    Task UpdateAsync(
        VolunteerApplication application);
}