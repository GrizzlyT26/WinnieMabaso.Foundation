using WinnieMabaso.Application.DTOs.Programmes;

namespace WinnieMabaso.Application.Interfaces;

public interface IProgrammeService
{
    Task<ProgrammeResponse> CreateAsync(
        string createdByUserId,
        CreateProgramme request);

    Task<List<ProgrammeResponse>> GetAllAsync();

    Task<List<ProgrammeResponse>> GetActiveAsync();

    Task<ProgrammeResponse?> GetByIdAsync(
        string id);

    Task UpdateAsync(
        string id,
        string updatedByUserId,
        CreateProgramme request);

    Task DeactivateAsync(
        string id,
        string updatedByUserId);

    Task ActivateAsync(
        string id,
        string updatedByUserId);

    Task DeleteAsync(
        string id);
}