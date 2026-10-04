using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.Interfaces;

public interface IProgrammeRepository
{
    Task<Programme?> GetByIdAsync(
        string id);

    Task<List<Programme>> GetAllAsync();

    Task<List<Programme>> GetActiveAsync();

    Task CreateAsync(
        Programme programme);

    Task UpdateAsync(
        Programme programme);

    Task DeleteAsync(
        string id);
}