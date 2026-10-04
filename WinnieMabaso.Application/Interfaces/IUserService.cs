using WinnieMabaso.Application.DTOs.Users;

namespace WinnieMabaso.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(string userId);

    Task<UserDto?> GetByFirebaseUidAsync(string firebaseUid);

    Task<UserDto> CreateAsync(UserDto user);

    Task UpdateAsync(UserDto user);
}