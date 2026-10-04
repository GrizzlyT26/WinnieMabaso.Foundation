using WinnieMabaso.Domain.Enums;

namespace WinnieMabaso.Application.DTOs.Users;

public class UserProfileResponse
{
    public string Id { get; set; } = string.Empty;

    public string FirebaseUid { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public UserStatus Status { get; set; }

    public bool PrivacyNoticeAccepted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}