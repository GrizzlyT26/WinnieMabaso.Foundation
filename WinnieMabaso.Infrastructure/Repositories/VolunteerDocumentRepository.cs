using Google.Cloud.Firestore;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Repositories;

public class VolunteerDocumentRepository
    : IVolunteerDocumentRepository
{
    private readonly FirestoreDb _firestore;

    private const string CollectionName =
        "volunteerDocuments";

    public VolunteerDocumentRepository(
        FirestoreDb firestore)
    {
        _firestore = firestore;
    }


    // ==================================================
    // GET DOCUMENT BY ID
    // ==================================================

    public async Task<VolunteerDocument?>
        GetByIdAsync(
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

        return snapshot
            .ConvertTo<VolunteerDocument>();
    }


    // ==================================================
    // GET DOCUMENTS BY VOLUNTEER USER ID
    // ==================================================

    public async Task<List<VolunteerDocument>>
        GetByVolunteerUserIdAsync(
            string volunteerUserId)
    {
        if (string.IsNullOrWhiteSpace(
            volunteerUserId))
        {
            return new List<VolunteerDocument>();
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
                document.ConvertTo<VolunteerDocument>())
            .OrderByDescending(document =>
                document.UploadedAt)
            .ToList();
    }


    // ==================================================
    // GET DOCUMENTS BY APPLICATION ID
    // ==================================================

    public async Task<List<VolunteerDocument>>
        GetByApplicationIdAsync(
            string volunteerApplicationId)
    {
        if (string.IsNullOrWhiteSpace(
            volunteerApplicationId))
        {
            return new List<VolunteerDocument>();
        }

        Query query =
            _firestore
                .Collection(CollectionName)
                .WhereEqualTo(
                    "VolunteerApplicationId",
                    volunteerApplicationId);

        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();

        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<VolunteerDocument>())
            .OrderByDescending(document =>
                document.UploadedAt)
            .ToList();
    }


    // ==================================================
    // CREATE DOCUMENT
    // ==================================================

    public async Task CreateAsync(
        VolunteerDocument document)
    {
        if (document == null)
        {
            throw new ArgumentNullException(
                nameof(document));
        }

        if (string.IsNullOrWhiteSpace(
            document.Id))
        {
            document.Id =
                Guid.NewGuid().ToString();
        }

        DocumentReference documentReference =
            _firestore
                .Collection(CollectionName)
                .Document(document.Id);

        await documentReference.SetAsync(
            document);
    }


    // ==================================================
    // DELETE DOCUMENT METADATA
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