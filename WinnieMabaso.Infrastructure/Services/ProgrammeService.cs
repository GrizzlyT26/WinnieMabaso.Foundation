using WinnieMabaso.Application.DTOs.Programmes;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Services;

public class ProgrammeService
    : IProgrammeService
{
    private readonly IProgrammeRepository _repository;


    public ProgrammeService(
        IProgrammeRepository repository)
    {
        _repository = repository;
    }


    // ==================================================
    // CREATE PROGRAMME
    // ==================================================

    public async Task<ProgrammeResponse> CreateAsync(
        string createdByUserId,
        CreateProgramme request)
    {
        if (string.IsNullOrWhiteSpace(
            createdByUserId))
        {
            throw new ArgumentException(
                "Created by user ID is required.");
        }


        if (request == null)
        {
            throw new ArgumentNullException(
                nameof(request));
        }


        ValidateProgrammeDates(
            request.StartDate,
            request.EndDate);


        if (request.Capacity <= 0)
        {
            throw new ArgumentException(
                "Programme capacity must be greater than zero.");
        }


        DateTime now =
            DateTime.UtcNow;


        Programme programme =
            new Programme
            {
                Id =
                    Guid.NewGuid().ToString(),

                Name =
                    request.Name.Trim(),

                Description =
                    request.Description.Trim(),

                Category =
                    request.Category.Trim(),

                Location =
                    request.Location.Trim(),

                StartDate =
                    request.StartDate,

                EndDate =
                    request.EndDate,

                Capacity =
                    request.Capacity,

                IsActive =
                    true,

                CreatedAt =
                    now,

                UpdatedAt =
                    now,

                CreatedByUserId =
                    createdByUserId,

                UpdatedByUserId =
                    createdByUserId
            };


        await _repository.CreateAsync(
            programme);


        return MapToResponse(
            programme);
    }


    // ==================================================
    // GET ALL PROGRAMMES
    // ==================================================

    public async Task<List<ProgrammeResponse>>
        GetAllAsync()
    {
        List<Programme> programmes =
            await _repository.GetAllAsync();

        return programmes
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // GET ACTIVE PROGRAMMES
    // ==================================================

    public async Task<List<ProgrammeResponse>>
        GetActiveAsync()
    {
        List<Programme> programmes =
            await _repository.GetActiveAsync();

        return programmes
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // GET PROGRAMME BY ID
    // ==================================================

    public async Task<ProgrammeResponse?>
        GetByIdAsync(
            string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }


        Programme? programme =
            await _repository.GetByIdAsync(
                id);

        if (programme == null)
        {
            return null;
        }


        return MapToResponse(
            programme);
    }


    // ==================================================
    // UPDATE PROGRAMME
    // ==================================================

    public async Task UpdateAsync(
        string id,
        string updatedByUserId,
        CreateProgramme request)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                "Programme ID is required.");
        }


        if (string.IsNullOrWhiteSpace(
            updatedByUserId))
        {
            throw new ArgumentException(
                "Updated by user ID is required.");
        }


        if (request == null)
        {
            throw new ArgumentNullException(
                nameof(request));
        }


        ValidateProgrammeDates(
            request.StartDate,
            request.EndDate);


        if (request.Capacity <= 0)
        {
            throw new ArgumentException(
                "Programme capacity must be greater than zero.");
        }


        Programme? programme =
            await _repository.GetByIdAsync(
                id);

        if (programme == null)
        {
            throw new KeyNotFoundException(
                "Programme could not be found.");
        }


        programme.Name =
            request.Name.Trim();

        programme.Description =
            request.Description.Trim();

        programme.Category =
            request.Category.Trim();

        programme.Location =
            request.Location.Trim();

        programme.StartDate =
            request.StartDate;

        programme.EndDate =
            request.EndDate;

        programme.Capacity =
            request.Capacity;

        programme.UpdatedAt =
            DateTime.UtcNow;

        programme.UpdatedByUserId =
            updatedByUserId;


        await _repository.UpdateAsync(
            programme);
    }


    // ==================================================
    // DEACTIVATE PROGRAMME
    // ==================================================

    public async Task DeactivateAsync(
        string id,
        string updatedByUserId)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                "Programme ID is required.");
        }


        if (string.IsNullOrWhiteSpace(
            updatedByUserId))
        {
            throw new ArgumentException(
                "Updated by user ID is required.");
        }


        Programme? programme =
            await _repository.GetByIdAsync(
                id);

        if (programme == null)
        {
            throw new KeyNotFoundException(
                "Programme could not be found.");
        }


        programme.IsActive =
            false;

        programme.UpdatedAt =
            DateTime.UtcNow;

        programme.UpdatedByUserId =
            updatedByUserId;


        await _repository.UpdateAsync(
            programme);
    }


    // ==================================================
    // ACTIVATE PROGRAMME
    // ==================================================

    public async Task ActivateAsync(
        string id,
        string updatedByUserId)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                "Programme ID is required.");
        }


        if (string.IsNullOrWhiteSpace(
            updatedByUserId))
        {
            throw new ArgumentException(
                "Updated by user ID is required.");
        }


        Programme? programme =
            await _repository.GetByIdAsync(
                id);

        if (programme == null)
        {
            throw new KeyNotFoundException(
                "Programme could not be found.");
        }


        programme.IsActive =
            true;

        programme.UpdatedAt =
            DateTime.UtcNow;

        programme.UpdatedByUserId =
            updatedByUserId;


        await _repository.UpdateAsync(
            programme);
    }


    // ==================================================
    // DELETE PROGRAMME
    // ==================================================

    public async Task DeleteAsync(
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                "Programme ID is required.");
        }


        Programme? programme =
            await _repository.GetByIdAsync(
                id);

        if (programme == null)
        {
            throw new KeyNotFoundException(
                "Programme could not be found.");
        }


        await _repository.DeleteAsync(
            id);
    }


    // ==================================================
    // VALIDATE PROGRAMME DATES
    // ==================================================

    private static void ValidateProgrammeDates(
        DateTime startDate,
        DateTime? endDate)
    {
        if (endDate.HasValue &&
            endDate.Value < startDate)
        {
            throw new ArgumentException(
                "Programme end date cannot be before the start date.");
        }
    }


    // ==================================================
    // MAP ENTITY TO RESPONSE
    // ==================================================

    private static ProgrammeResponse
        MapToResponse(
            Programme programme)
    {
        return new ProgrammeResponse
        {
            Id =
                programme.Id,

            Name =
                programme.Name,

            Description =
                programme.Description,

            Category =
                programme.Category,

            Location =
                programme.Location,

            StartDate =
                programme.StartDate,

            EndDate =
                programme.EndDate,

            Capacity =
                programme.Capacity,

            IsActive =
                programme.IsActive,

            CreatedAt =
                programme.CreatedAt,

            UpdatedAt =
                programme.UpdatedAt,

            CreatedByUserId =
                programme.CreatedByUserId,

            UpdatedByUserId =
                programme.UpdatedByUserId
        };
    }
}