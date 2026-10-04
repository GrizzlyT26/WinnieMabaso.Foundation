using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace WinnieMabaso.Infrastructure.Firebase;

public class FirebaseInitializer
{
    public FirebaseApp App { get; }
    public FirestoreDb Firestore { get; }

    public FirebaseInitializer()
    {
        App = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.GetApplicationDefault(),
            ProjectId = "winnie-mabaso-foundation"
        });

        Firestore = FirestoreDb.Create("winnie-mabaso-foundation");
    }
}