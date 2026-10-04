using WinnieMabaso.Application.DTOs.Users;
using WinnieMabaso.Application.Interfaces;
using WinnieMabaso.Domain.Entities;
using WinnieMabaso.Infrastructure.Repositories;

namespace WinnieMabaso.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // ==================================================
    // GET USER BY ID
    // ==================================================

    public async Task<UserDto?> GetByIdAsync(
        string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        User? user =
            await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        return MapToDto(user);
    }

    // ==================================================
    // GET USER BY FIREBASE UID
    // ==================================================

    public async Task<UserDto?> GetByFirebaseUidAsync(
        string firebaseUid)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
        {
            return null;
        }

        User? user =
            await _userRepository.GetByFirebaseUidAsync(
                firebaseUid);

        if (user == null)
        {
            return null;
        }

        return MapToDto(user);
    }

    // ==================================================
    // CREATE USER
    // ==================================================

    public async Task<UserDto> CreateAsync(
        UserDto userDto)
    {
        if (userDto == null)
        {
            throw new ArgumentNullException(
                nameof(userDto));
        }

        DateTime currentUtcTime =
            DateTime.UtcNow;

        User user = new User
        {
            Id =
                string.IsNullOrWhiteSpace(userDto.Id)
                    ? Guid.NewGuid().ToString()
                    : userDto.Id,

            FirebaseUid =
                userDto.FirebaseUid,

            FullName =
                userDto.FullName,

            Email =
                userDto.Email,

            MobileNumber =
                userDto.MobileNumber ?? string.Empty,

            Role =
                userDto.Role,

            Status =
                userDto.Status,

            PrivacyNoticeAccepted =
                userDto.PrivacyNoticeAccepted,

            CreatedAt =
                userDto.CreatedAt == default
                    ? currentUtcTime
                    : userDto.CreatedAt,

            UpdatedAt =
                currentUtcTime
        };

        await _userRepository.CreateAsync(user);

        return MapToDto(user);
    }

    // ==================================================
    // UPDATE USER
    // ==================================================

    public async Task UpdateAsync(
        UserDto userDto)
    {
        if (userDto == null)
        {
            throw new ArgumentNullException(
                nameof(userDto));
        }

        if (string.IsNullOrWhiteSpace(userDto.Id))
        {
            throw new InvalidOperationException(
                "User ID is required.");
        }

        User? existingUser =
            await _userRepository.GetByIdAsync(
                userDto.Id);

        if (existingUser == null)
        {
            throw new InvalidOperationException(
                "User profile could not be found.");
        }

        // --------------------------------------------------
        // Update user profile information
        // --------------------------------------------------

        existingUser.FullName =
            userDto.FullName.Trim();

        existingUser.MobileNumber =
            userDto.MobileNumber?.Trim() ?? string.Empty;

        existingUser.UpdatedAt =
            DateTime.UtcNow;

        // --------------------------------------------------
        // IMPORTANT:
        // Do not allow normal profile updates to change:
        //
        // FirebaseUid
        // Email
        // Role
        // Status
        // CreatedAt
        // --------------------------------------------------

        await _userRepository.UpdateAsync(
            existingUser);
    }

    // ==================================================
    // MAP USER ENTITY TO USER DTO
    // ==================================================

    private static UserDto MapToDto(
        User user)
    {
        return new UserDto
        {
            Id =
                user.Id,

            FirebaseUid =
                user.FirebaseUid,

            FullName =
                user.FullName,

            Email =
                user.Email,

            MobileNumber =
                string.IsNullOrWhiteSpace(
                    user.MobileNumber)
                    ? null
                    : user.MobileNumber,

            Role =
                user.Role,

            Status =
                user.Status,

            PrivacyNoticeAccepted =
                user.PrivacyNoticeAccepted,

            CreatedAt =
                user.CreatedAt

            // VerifiedAt is currently not mapped
            // because the User entity does not
            // currently contain a VerifiedAt property.
        };
    }
}