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

    public async Task<IServiceResult> CreateAsync(User user, string password, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateUserAsync(user, cancellationToken);

        if (validationResult.HasErrors())
        {
            return validationResult;
        }

        user.PasswordHash = HashPassword(password);

        return await GetUserCreationResultAsync(user, password, cancellationToken);
    }

    public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        return _usersRepository.UpdateAsync(user, cancellationToken);
    }

    public Task<User?> FindById(Guid id, CancellationToken cancellationToken)
    {
        return _usersRepository.GetAsync(id, cancellationToken);
    }

    private async Task<IServiceResult> ValidateUserAsync(User user, CancellationToken cancellationToken)
    {
        var validation = new ServiceValidationResult();
        var findUserWithEmail = await _usersRepository.FindByEmailAsync(user.Email, cancellationToken);

        if (findUserWithEmail != null)
        {
            validation.Errors.Add("A user with same email already exists");
        }

        return validation;
    }

    private async Task<IServiceResult> GetUserCreationResultAsync(User user, string password, CancellationToken cancellationToken)
    {
        var createdUser = await _usersRepository.CreateAsync(user, cancellationToken);

        if (createdUser is null)
        {
            return new ServiceResult<User>("Failed creating User");
        }

        return new ServiceResult<User>(createdUser);
    }

    private static string HashPassword(string password)
    {
        return password;
    }
}
