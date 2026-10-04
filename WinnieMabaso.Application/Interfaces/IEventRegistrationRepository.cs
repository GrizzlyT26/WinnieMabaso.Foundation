using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.Interfaces;

public interface IEventRegistrationRepository
{
    Task<EventRegistration?> GetByIdAsync(string id);

    Task<EventRegistration?> GetByEventAndUserAsync(
        string eventId,
        string userId);

    Task<List<EventRegistration>> GetByEventAsync(
        string eventId);

    Task<List<EventRegistration>> GetByUserAsync(
        string userId);

    Task<int> GetActiveRegistrationCountAsync(
        string eventId);

    Task CreateAsync(
        EventRegistration registration);

    Task UpdateAsync(
        EventRegistration registration);
}