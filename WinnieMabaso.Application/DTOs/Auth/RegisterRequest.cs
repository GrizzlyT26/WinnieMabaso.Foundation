namespace WinnieMabaso.Application.DTOs.Auth;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? MobileNumber { get; set; }

    public int Role { get; set; }

    public bool PrivacyNoticeAccepted { get; set; }
}