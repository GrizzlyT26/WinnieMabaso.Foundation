using WinnieMabaso.Application.DTOs.Auth;
using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Application.Interfaces;

public interface IAuthService
{
    Task<bool> UserExistsAsync(string email);

    Task<User> RegisterAsync(RegisterRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);
}