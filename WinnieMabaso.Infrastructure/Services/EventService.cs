using WinnieMabaso.Application.DTOs.Events;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;


    public EventService(
        IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }


    // ==================================================
    // CREATE EVENT
    // ==================================================

    public async Task<EventResponse> CreateAsync(
        string createdByUserId,
        CreateEvent request)
    {
        if (string.IsNullOrWhiteSpace(createdByUserId))
        {
            throw new ArgumentException(
                "Created by user ID is required.");
        }


        ValidateEventDates(
            request.StartsAt,
            request.EndsAt,
            request.RegistrationDeadline);


        ValidateEligibleRoles(
            request.EligibleRoles);


        Event eventItem = new Event
        {
            Id = Guid.NewGuid().ToString(),

            Title = request.Title.Trim(),

            Description = request.Description.Trim(),

            StartsAt = request.StartsAt,

            EndsAt = request.EndsAt,

            Location = request.Location.Trim(),

            Capacity = request.Capacity,

            RegistrationDeadline =
                request.RegistrationDeadline,

            EligibleRoles =
                request.EligibleRoles
                    .Select(role => role.Trim())
                    .Where(role =>
                        !string.IsNullOrWhiteSpace(role))
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .ToList(),

            Published = request.Published,

            Cancelled = false,

            CreatedAt = DateTime.UtcNow,

            UpdatedAt = DateTime.UtcNow,

            CreatedByUserId = createdByUserId,

            UpdatedByUserId = createdByUserId
        };


        await _eventRepository.CreateAsync(
            eventItem);


        return MapToResponse(
            eventItem);
    }


    // ==================================================
    // GET EVENT BY ID
    // ==================================================

    public async Task<EventResponse?> GetByIdAsync(
        string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
        {
            return null;
        }


        Event? eventItem =
            await _eventRepository
                .GetByIdAsync(eventId);


        if (eventItem == null)
        {
            return null;
        }


        return MapToResponse(
            eventItem);
    }


    // ==================================================
    // GET ALL EVENTS
    // ==================================================

    public async Task<List<EventResponse>> GetAllAsync()
    {
        List<Event> events =
            await _eventRepository
                .GetAllAsync();


        return events
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // GET PUBLISHED EVENTS
    // ==================================================

    public async Task<List<EventResponse>>
        GetPublishedAsync()
    {
        List<Event> events =
            await _eventRepository
                .GetPublishedAsync();


        return events
            .Where(eventItem =>
                !eventItem.Cancelled)
            .Select(MapToResponse)
            .ToList();
    }


    // ==================================================
    // UPDATE EVENT
    // ==================================================

    public async Task<EventResponse> UpdateAsync(
        string eventId,
        string updatedByUserId,
        UpdateEvent request)
    {
        if (string.IsNullOrWhiteSpace(eventId))
        {
            throw new ArgumentException(
                "Event ID is required.");
        }


        if (string.IsNullOrWhiteSpace(
            updatedByUserId))
        {
            throw new ArgumentException(
                "Updated by user ID is required.");
        }


        Event? existingEvent =
            await _eventRepository
                .GetByIdAsync(eventId);


        if (existingEvent == null)
        {
            throw new KeyNotFoundException(
                "Event could not be found.");
        }


        if (existingEvent.Cancelled)
        {
            throw new ArgumentException(
                "A cancelled event cannot be updated.");
        }


        ValidateEventDates(
            request.StartsAt,
            request.EndsAt,
            request.RegistrationDeadline);


        ValidateEligibleRoles(
            request.EligibleRoles);


        existingEvent.Title =
            request.Title.Trim();


        existingEvent.Description =
            request.Description.Trim();


        existingEvent.StartsAt =
            request.StartsAt;


        existingEvent.EndsAt =
            request.EndsAt;


        existingEvent.Location =
            request.Location.Trim();


        existingEvent.Capacity =
            request.Capacity;


        existingEvent.RegistrationDeadline =
            request.RegistrationDeadline;


        existingEvent.EligibleRoles =
            request.EligibleRoles
                .Select(role => role.Trim())
                .Where(role =>
                    !string.IsNullOrWhiteSpace(role))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();


        existingEvent.UpdatedAt =
            DateTime.UtcNow;


        existingEvent.UpdatedByUserId =
            updatedByUserId;


        await _eventRepository.UpdateAsync(
            existingEvent);


        return MapToResponse(
            existingEvent);
    }


    // ==================================================
    // PUBLISH / UNPUBLISH EVENT
    // ==================================================

    public async Task UpdatePublishedStatusAsync(
        string eventId,
        bool published,
        string updatedByUserId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
        {
            throw new ArgumentException(
                "Event ID is required.");
        }


        if (string.IsNullOrWhiteSpace(
            updatedByUserId))
        {
            throw new ArgumentException(
                "Updated by user ID is required.");
        }


        Event? existingEvent =
            await _eventRepository
                .GetByIdAsync(eventId);


        if (existingEvent == null)
        {
            throw new KeyNotFoundException(
                "Event could not be found.");
        }


        if (existingEvent.Cancelled &&
            published)
        {
            throw new ArgumentException(
                "A cancelled event cannot be published.");
        }


        existingEvent.Published =
            published;


        existingEvent.UpdatedAt =
            DateTime.UtcNow;


        existingEvent.UpdatedByUserId =
            updatedByUserId;


        await _eventRepository.UpdateAsync(
            existingEvent);
    }


    // ==================================================
    // DELETE EVENT
    // ==================================================

    public async Task DeleteAsync(
        string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
        {
            throw new ArgumentException(
                "Event ID is required.");
        }


        Event? existingEvent =
            await _eventRepository
                .GetByIdAsync(eventId);


        if (existingEvent == null)
        {
            throw new KeyNotFoundException(
                "Event could not be found.");
        }


        await _eventRepository.DeleteAsync(
            eventId);
    }


    // ==================================================
    // VALIDATE EVENT DATES
    // ==================================================

    private static void ValidateEventDates(
        DateTime startsAt,
        DateTime? endsAt,
        DateTime registrationDeadline)
    {
        if (endsAt.HasValue &&
            endsAt.Value <= startsAt)
        {
            throw new ArgumentException(
                "Event end date must be after the start date.");
        }


        if (registrationDeadline >= startsAt)
        {
            throw new ArgumentException(
                "Registration deadline must be before the event starts.");
        }
    }


    // ==================================================
    // VALIDATE ELIGIBLE ROLES
    // ==================================================

    private static void ValidateEligibleRoles(
        List<string>? eligibleRoles)
    {
        if (eligibleRoles == null ||
            eligibleRoles.Count == 0)
        {
            throw new ArgumentException(
                "At least one eligible role is required.");
        }


        string[] allowedRoles =
        {
            "Beneficiary",
            "Volunteer",
            "Donor"
        };


        foreach (string role in eligibleRoles)
        {
            if (!allowedRoles.Contains(
                    role.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"Invalid eligible role: {role}. " +
                    "Allowed roles are Beneficiary, Volunteer and Donor.");
            }
        }
    }


    // ==================================================
    // MAP ENTITY TO RESPONSE
    // ==================================================

    private static EventResponse MapToResponse(
        Event eventItem)
    {
        return new EventResponse
        {
            Id = eventItem.Id,

            Title = eventItem.Title,

            Description = eventItem.Description,

            StartsAt = eventItem.StartsAt,

            EndsAt = eventItem.EndsAt,

            Location = eventItem.Location,

            Capacity = eventItem.Capacity,

            RegistrationDeadline =
                eventItem.RegistrationDeadline,

            EligibleRoles =
                eventItem.EligibleRoles
                    .ToList(),

            Published =
                eventItem.Published,

            Cancelled =
                eventItem.Cancelled,

            RegisteredCount = 0,

            IsFull = false,

            CreatedAt =
                eventItem.CreatedAt,

            UpdatedAt =
                eventItem.UpdatedAt,

            CreatedByUserId =
                eventItem.CreatedByUserId,

            UpdatedByUserId =
                eventItem.UpdatedByUserId
        };
    }
}