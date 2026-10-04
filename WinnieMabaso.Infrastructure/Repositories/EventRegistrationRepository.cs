using Google.Cloud.Firestore;

using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;
using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Infrastructure.Repositories;

public class EventRegistrationRepository : IEventRegistrationRepository
{
    private readonly FirestoreDb _firestoreDb;
    private readonly CollectionReference _registrationsCollection;

    public EventRegistrationRepository(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
        _registrationsCollection =
            _firestoreDb.Collection("eventRegistrations");
    }

    public async Task<EventRegistration?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        DocumentSnapshot snapshot =
            await _registrationsCollection
                .Document(id)
                .GetSnapshotAsync();

        if (!snapshot.Exists)
            return null;

        return snapshot.ConvertTo<EventRegistration>();
    }

    public async Task<EventRegistration?> GetByEventAndUserAsync(
        string eventId,
        string userId)
    {
        if (string.IsNullOrWhiteSpace(eventId) ||
            string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        QuerySnapshot snapshot =
            await _registrationsCollection
                .WhereEqualTo("EventId", eventId)
                .WhereEqualTo("UserId", userId)
                .Limit(1)
                .GetSnapshotAsync();

        return snapshot.Documents.Count == 0
            ? null
            : snapshot.Documents[0]
                .ConvertTo<EventRegistration>();
    }

    public async Task<List<EventRegistration>> GetByEventAsync(
        string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            return new List<EventRegistration>();

        QuerySnapshot snapshot =
            await _registrationsCollection
                .WhereEqualTo("EventId", eventId)
                .OrderBy("RegisteredAt")
                .GetSnapshotAsync();

        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<EventRegistration>())
            .ToList();
    }

    public async Task<List<EventRegistration>> GetByUserAsync(
        string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return new List<EventRegistration>();

        QuerySnapshot snapshot =
            await _registrationsCollection
                .WhereEqualTo("UserId", userId)
                .OrderByDescending("RegisteredAt")
                .GetSnapshotAsync();

        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<EventRegistration>())
            .ToList();
    }

    public async Task<int> GetActiveRegistrationCountAsync(
        string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            return 0;

        QuerySnapshot snapshot =
            await _registrationsCollection
                .WhereEqualTo("EventId", eventId)
                .WhereEqualTo(
                    "Status",
                    EventRegistrationStatus.Registered)
                .GetSnapshotAsync();

        return snapshot.Documents.Count;
    }

    public async Task CreateAsync(
        EventRegistration registration)
    {
        if (string.IsNullOrWhiteSpace(registration.Id))
            registration.Id = Guid.NewGuid().ToString();

        await _registrationsCollection
            .Document(registration.Id)
            .SetAsync(registration);
    }

    public async Task UpdateAsync(
        EventRegistration registration)
    {
        if (string.IsNullOrWhiteSpace(registration.Id))
            throw new ArgumentException(
                "Registration ID is required.");

        await _registrationsCollection
            .Document(registration.Id)
            .SetAsync(
                registration,
                SetOptions.MergeAll);
    }
}