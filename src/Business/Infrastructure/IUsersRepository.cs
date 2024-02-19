using Business.Models;

namespace Business.Infrastructure;

public interface IUsersRepository
{
    Task<User?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> CreateAsync(User user, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByEmailAndPasswordAsync(string email, string passwordHash, CancellationToken cancellationToken);
}
