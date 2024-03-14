using Business.Infrastructure;
using Business.Models;

namespace Business.Services.UserServices;

public class UserService : IUserService
{
    private readonly IUsersRepository _usersRepository;

    public UserService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<User?> LoginUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        return await _usersRepository.GetByEmailAndPasswordAsync(email, HashPassword(password), cancellationToken);
    }

    public async Task<User?> CreateAsync(User user, string password, CancellationToken cancellationToken)
    {
        user.PasswordHash = HashPassword(password);

        return await _usersRepository.CreateAsync(user, cancellationToken);
    }

    public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        return _usersRepository.UpdateAsync(user, cancellationToken);
    }

    public Task<User?> FindById(Guid id, CancellationToken cancellationToken)
    {
        return _usersRepository.GetAsync(id, cancellationToken);
    }

    public async Task<IServiceResult> ValidateUserAsync(User user, CancellationToken cancellationToken)
    {
        var validation = new ServiceValidationResult();
        var findUserWithEmail = await _usersRepository.FindByEmailAsync(user.Email, cancellationToken);

        if (findUserWithEmail != null)
        {
            validation.ValidationErrors.Add("A user with same email already exists");
        }

        return validation;
    }

    private static string HashPassword(string password)
    {
        return password;
    }
}
