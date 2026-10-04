using WinnieMabaso.Application.DTOs.Events;

namespace WinnieMabaso.Application.Interfaces;

public interface IEventService
{
    // ==================================================
    // CREATE EVENT
    // ==================================================

    Task<EventResponse> CreateAsync(
        string createdByUserId,
        CreateEvent request);


    // ==================================================
    // GET EVENT BY ID
    // ==================================================

    Task<EventResponse?> GetByIdAsync(
        string eventId);


    // ==================================================
    // GET ALL EVENTS
    // ==================================================

    Task<List<EventResponse>> GetAllAsync();


    // ==================================================
    // GET PUBLISHED EVENTS
    // ==================================================

    Task<List<EventResponse>> GetPublishedAsync();


    // ==================================================
    // UPDATE EVENT
    // ==================================================

    Task<EventResponse> UpdateAsync(
        string eventId,
        string updatedByUserId,
        UpdateEvent request);


    // ==================================================
    // PUBLISH / UNPUBLISH EVENT
    // ==================================================

    Task UpdatePublishedStatusAsync(
        string eventId,
        bool published,
        string updatedByUserId);


    // ==================================================
    // DELETE EVENT
    // ==================================================

    Task DeleteAsync(
        string eventId);
}