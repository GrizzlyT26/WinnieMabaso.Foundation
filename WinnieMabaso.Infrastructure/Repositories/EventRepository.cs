using Google.Cloud.Firestore;

using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly FirestoreDb _firestoreDb;

    private readonly CollectionReference _eventsCollection;


    public EventRepository(
        FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;

        _eventsCollection =
            _firestoreDb
                .Collection("events");
    }


    // ==================================================
    // GET EVENT BY ID
    // ==================================================

    public async Task<Event?> GetByIdAsync(
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }


        DocumentSnapshot snapshot =
            await _eventsCollection
                .Document(id)
                .GetSnapshotAsync();


        if (!snapshot.Exists)
        {
            return null;
        }


        return snapshot.ConvertTo<Event>();
    }


    // ==================================================
    // GET ALL EVENTS
    // ==================================================

    public async Task<List<Event>> GetAllAsync()
    {
        QuerySnapshot snapshot =
            await _eventsCollection
                .OrderByDescending("StartsAt")
                .GetSnapshotAsync();


        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<Event>())
            .ToList();
    }


    // ==================================================
    // GET PUBLISHED EVENTS
    // ==================================================

    public async Task<List<Event>> GetPublishedAsync()
    {
        QuerySnapshot snapshot =
            await _eventsCollection
                .WhereEqualTo(
                    "Published",
                    true)
                .OrderBy("StartsAt")
                .GetSnapshotAsync();


        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<Event>())
            .ToList();
    }


    // ==================================================
    // CREATE EVENT
    // ==================================================

    public async Task CreateAsync(
        Event eventItem)
    {
        if (string.IsNullOrWhiteSpace(
            eventItem.Id))
        {
            eventItem.Id =
                Guid.NewGuid().ToString();
        }


        await _eventsCollection
            .Document(eventItem.Id)
            .SetAsync(eventItem);
    }


    // ==================================================
    // UPDATE EVENT
    // ==================================================

    public async Task UpdateAsync(
        Event eventItem)
    {
        if (string.IsNullOrWhiteSpace(
            eventItem.Id))
        {
            throw new ArgumentException(
                "Event ID is required.");
        }


        await _eventsCollection
            .Document(eventItem.Id)
            .SetAsync(
                eventItem,
                SetOptions.MergeAll);
    }


    // ==================================================
    // DELETE EVENT
    // ==================================================

    public async Task DeleteAsync(
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }


        await _eventsCollection
            .Document(id)
            .DeleteAsync();
    }
}