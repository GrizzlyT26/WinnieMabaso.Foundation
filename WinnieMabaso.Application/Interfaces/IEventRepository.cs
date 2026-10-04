using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(
        string id);

    Task<List<Event>> GetAllAsync();

    Task<List<Event>> GetPublishedAsync();

    Task CreateAsync(
        Event eventItem);

    Task UpdateAsync(
        Event eventItem);

    Task DeleteAsync(
        string id);
}