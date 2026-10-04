using Google.Cloud.Firestore;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FirestoreDb _firestore;

    private const string CollectionName = "users";

    public UserRepository(FirestoreDb firestore)
    {
        _firestore = firestore;
    }

    // ==================================================
    // GET USER BY APPLICATION ID
    // ==================================================

    public async Task<User?> GetByIdAsync(string id)
    {
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

        return snapshot.ConvertTo<User>();
    }

    // ==================================================
    // GET USER BY EMAIL
    // ==================================================

    public async Task<User?> GetByEmailAsync(string email)
    {
        Query query =
            _firestore
                .Collection(CollectionName)
                .WhereEqualTo("Email", email);

        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();

        if (snapshot.Count == 0)
        {
            return null;
        }

        DocumentSnapshot document =
            snapshot.Documents[0];

        return document.ConvertTo<User>();
    }

    // ==================================================
    // GET USER BY FIREBASE UID
    // ==================================================

    public async Task<User?> GetByFirebaseUidAsync(
        string firebaseUid)
    {
        Query query =
            _firestore
                .Collection(CollectionName)
                .WhereEqualTo(
                    "FirebaseUid",
                    firebaseUid);

        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();

        if (snapshot.Count == 0)
        {
            return null;
        }

        DocumentSnapshot document =
            snapshot.Documents[0];

        return document.ConvertTo<User>();
    }

    // ==================================================
    // CREATE USER
    // ==================================================

    public async Task CreateAsync(User user)
    {
        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(user.Id);

        await document.SetAsync(user);
    }

    // ==================================================
    // UPDATE USER
    // ==================================================

    public async Task UpdateAsync(User user)
    {
        DocumentReference document =
            _firestore
                .Collection(CollectionName)
                .Document(user.Id);

        await document.SetAsync(
            user,
            SetOptions.MergeAll);
    }
}