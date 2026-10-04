using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.DTOs.Auth;

public class LoginResponse
{
    public string IdToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public string ExpiresIn { get; set; } = string.Empty;

    public WinnieMabaso.Domain.Entities.User User { get; set; } = null!;
}