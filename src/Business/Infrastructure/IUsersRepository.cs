using Business.Models;

namespace Business.Infrastructure;

public interface IUsersRepository : IRepository<User>
{
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByEmailAndPasswordAsync(string email, string passwordHash, CancellationToken cancellationToken);
}
