using Business.Models;

namespace Business.Services;

public interface IUserService
{
    Task<User?> CreateAsync(User user, string password, CancellationToken cancellationToken);
    Task<User?> FindById(Guid id, CancellationToken cancellationToken);
    Task<User?> LoginUserAsync(string username, string password, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
    Task<ValidationResult> ValidateUserAsync(User user, CancellationToken cancellationToken);
}
