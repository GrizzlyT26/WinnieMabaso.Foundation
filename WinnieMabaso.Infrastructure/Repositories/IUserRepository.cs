using WinnieMabaso.Domain.Entities;

namespace WinnieMabaso.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);

    Task<User?> GetByFirebaseUidAsync(string firebaseUid);

    Task<User?> GetByEmailAsync(string email);

    Task CreateAsync(User user);

    Task UpdateAsync(User user);
}