using Google.Cloud.Firestore;

namespace WinnieMabaso.Infrastructure.Firebase;

public class FirestoreService
{
    private readonly FirestoreDb _firestore;

    public FirestoreService(FirestoreDb firestore)
    {
        _firestore = firestore;
    }

    // --------------------------------------------------
    // Create / update document
    // --------------------------------------------------

    public async Task SetDocumentAsync(
        string collection,
        string documentId,
        object data)
    {
        DocumentReference document =
            _firestore
                .Collection(collection)
                .Document(documentId);

        await document.SetAsync(data);
    }

    // --------------------------------------------------
    // Get document
    // --------------------------------------------------

    public async Task<DocumentSnapshot> GetDocumentAsync(
        string collection,
        string documentId)
    {
        DocumentReference document =
            _firestore
                .Collection(collection)
                .Document(documentId);

        return await document.GetSnapshotAsync();
    }

    // --------------------------------------------------
    // Query collection
    // --------------------------------------------------

    public async Task<QuerySnapshot> QueryAsync(
        string collection,
        string field,
        object value)
    {
        Query query =
            _firestore
                .Collection(collection)
                .WhereEqualTo(field, value);

        return await query.GetSnapshotAsync();
    }
}