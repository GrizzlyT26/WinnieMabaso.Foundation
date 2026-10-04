using Google.Cloud.Firestore;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Repositories;

public class VolunteerApplicationRepository
    : IVolunteerApplicationRepository
{
    private readonly FirestoreDb _firestore;

    private const string CollectionName =
        "volunteerApplications";


    public VolunteerApplicationRepository(
        FirestoreDb firestore)
    {
        _firestore = firestore;
    }


    // ==================================================
    // GET APPLICATION BY ID
    // ==================================================

    public async Task<VolunteerApplication?>
        GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(id);

        DocumentSnapshot snapshot =
            await document.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return null;
        }

        return snapshot.ConvertTo<VolunteerApplication>();
    }


    // ==================================================
    // GET VOLUNTEER APPLICATIONS
    // ==================================================

    public async Task<List<VolunteerApplication>>
        GetByVolunteerUserIdAsync(
            string volunteerUserId)
    {
        if (string.IsNullOrWhiteSpace(
            volunteerUserId))
        {
            return new List<VolunteerApplication>();
        }

        Query query =
            _firestore
                .Collection(CollectionName)
                .WhereEqualTo(
                    "VolunteerUserId",
                    volunteerUserId);

        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();

        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<VolunteerApplication>())
            .OrderByDescending(application =>
                application.CreatedAt)
            .ToList();
    }


    // ==================================================
    // GET ALL APPLICATIONS
    // ==================================================

    public async Task<List<VolunteerApplication>>
        GetAllAsync()
    {
        QuerySnapshot snapshot =
            await _firestore
                .Collection(CollectionName)
                .GetSnapshotAsync();

        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<VolunteerApplication>())
            .OrderByDescending(application =>
                application.CreatedAt)
            .ToList();
    }


    // ==================================================
    // CREATE APPLICATION
    // ==================================================

    public async Task CreateAsync(
        VolunteerApplication application)
    {
        if (application == null)
        {
            throw new ArgumentNullException(
                nameof(application));
        }

        if (string.IsNullOrWhiteSpace(
            application.Id))
        {
            application.Id =
                Guid.NewGuid().ToString();
        }

        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(application.Id);

        await document.SetAsync(application);
    }


    // ==================================================
    // UPDATE APPLICATION
    // ==================================================

    public async Task UpdateAsync(
        VolunteerApplication application)
    {
        if (application == null)
        {
            throw new ArgumentNullException(
                nameof(application));
        }

        if (string.IsNullOrWhiteSpace(
            application.Id))
        {
            throw new InvalidOperationException(
                "Volunteer application ID is required.");
        }

        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(application.Id);

        await document.SetAsync(
            application,
            SetOptions.MergeAll);
    }
}