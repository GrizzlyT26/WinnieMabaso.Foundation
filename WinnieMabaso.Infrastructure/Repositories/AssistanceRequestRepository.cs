using Google.Cloud.Firestore;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Repositories;

public class AssistanceRequestRepository
    : IAssistanceRequestRepository
{
    private readonly FirestoreDb _firestore;

    private const string CollectionName =
        "assistanceRequests";


    public AssistanceRequestRepository(
        FirestoreDb firestore)
    {
        _firestore = firestore;
    }


    // ==================================================
    // GET BY ID
    // ==================================================

    public async Task<AssistanceRequest?>
        GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;


        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(id);


        DocumentSnapshot snapshot =
            await document.GetSnapshotAsync();


        if (!snapshot.Exists)
            return null;


        return snapshot.ConvertTo<AssistanceRequest>();
    }


    // ==================================================
    // GET BY REFERENCE
    // ==================================================

    public async Task<AssistanceRequest?>
        GetByReferenceAsync(
            string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
            return null;


        Query query =
            _firestore
                .Collection(CollectionName)
                .WhereEqualTo(
                    "Reference",
                    reference);


        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();


        if (snapshot.Count == 0)
            return null;


        return snapshot.Documents[0]
            .ConvertTo<AssistanceRequest>();
    }


    // ==================================================
    // GET BENEFICIARY REQUESTS
    // ==================================================

    public async Task<List<AssistanceRequest>>
        GetByBeneficiaryAsync(
            string beneficiaryUserId)
    {
        if (string.IsNullOrWhiteSpace(
            beneficiaryUserId))
        {
            return new List<AssistanceRequest>();
        }


        Query query =
            _firestore
                .Collection(CollectionName)
                .WhereEqualTo(
                    "BeneficiaryUserId",
                    beneficiaryUserId);


        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();


        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<AssistanceRequest>())
            .OrderByDescending(request =>
                request.CreatedAt)
            .ToList();
    }


    // ==================================================
    // GET ALL
    // ==================================================

    public async Task<List<AssistanceRequest>>
        GetAllAsync()
    {
        QuerySnapshot snapshot =
            await _firestore
                .Collection(CollectionName)
                .GetSnapshotAsync();


        return snapshot.Documents
            .Select(document =>
                document.ConvertTo<AssistanceRequest>())
            .OrderByDescending(request =>
                request.CreatedAt)
            .ToList();
    }


    // ==================================================
    // CREATE
    // ==================================================

    public async Task CreateAsync(
        AssistanceRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(
                nameof(request));


        if (string.IsNullOrWhiteSpace(
            request.Id))
        {
            request.Id =
                Guid.NewGuid().ToString();
        }


        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(request.Id);


        await document.SetAsync(request);
    }


    // ==================================================
    // UPDATE
    // ==================================================

    public async Task UpdateAsync(
        AssistanceRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(
                nameof(request));


        if (string.IsNullOrWhiteSpace(
            request.Id))
        {
            throw new InvalidOperationException(
                "Assistance request ID is required.");
        }


        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(request.Id);


        await document.SetAsync(
            request,
            SetOptions.MergeAll);
    }
}