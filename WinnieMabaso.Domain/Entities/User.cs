using WinnieMabaso.Domain.Enums;
using Google.Cloud.Firestore;


namespace WinnieMabaso.Domain.Entities
{
    [FirestoreData]
    public class User
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string FirebaseUid { get; set; } = string.Empty;

        [FirestoreProperty]
        public string FullName { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;

        [FirestoreProperty]
        public string MobileNumber { get; set; } = string.Empty;

        [FirestoreProperty]
        public UserRole Role { get; set; }

        [FirestoreProperty]
        public UserStatus Status { get; set; }

        [FirestoreProperty]
        public bool PrivacyNoticeAccepted { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; }
    }
}