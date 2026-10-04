using Google.Cloud.Firestore;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Repositories;

public class ProgrammeRepository
    : IProgrammeRepository
{
    private readonly FirestoreDb _firestore;

    private const string CollectionName =
        "programmes";


    public ProgrammeRepository(
        FirestoreDb firestore)
    {
        _firestore = firestore;
    }


    // ==================================================
    // GET BY ID
    // ==================================================

    public async Task<Programme?> GetByIdAsync(
        string id)
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

        return snapshot.ConvertTo<Programme>();
    }


    // ==================================================
    // GET ALL
    // ==================================================

    public async Task<List<Programme>> GetAllAsync()
    {
        Query query =
            _firestore
                .Collection(CollectionName);

        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();

        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<Programme>())
            .OrderByDescending(
                programme => programme.CreatedAt)
            .ToList();
    }


    // ==================================================
    // GET ACTIVE PROGRAMMES
    // ==================================================

    public async Task<List<Programme>> GetActiveAsync()
    {
        Query query =
            _firestore
                .Collection(CollectionName)
                .WhereEqualTo(
                    "IsActive",
                    true);

        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();

        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<Programme>())
            .OrderBy(
                programme => programme.StartDate)
            .ToList();
    }


    // ==================================================
    // CREATE
    // ==================================================

    public async Task CreateAsync(
        Programme programme)
    {
        if (programme == null)
        {
            throw new ArgumentNullException(
                nameof(programme));
        }

        if (string.IsNullOrWhiteSpace(
            programme.Id))
        {
            programme.Id =
                Guid.NewGuid().ToString();
        }

        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(programme.Id);

        await document.SetAsync(
            programme);
    }


    // ==================================================
    // UPDATE
    // ==================================================

    public async Task UpdateAsync(
        Programme programme)
    {
        if (programme == null)
        {
            throw new ArgumentNullException(
                nameof(programme));
        }

        if (string.IsNullOrWhiteSpace(
            programme.Id))
        {
            throw new ArgumentException(
                "Programme ID is required.",
                nameof(programme));
        }

        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(programme.Id);

        await document.SetAsync(
            programme,
            SetOptions.MergeAll);
    }


    // ==================================================
    // DELETE
    // ==================================================

    public async Task DeleteAsync(
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(id);

        await document.DeleteAsync();
    }
}