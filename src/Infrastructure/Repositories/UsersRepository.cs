using Business;
using Business.Infrastructure;
using Business.Models;
using Infrastructure.Mappers;
using Infrastructure.Adapters;

namespace Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly IDbConnectionAdapter _dbConnectionAdapter;
    private readonly IDateTime _dateTime;

    public UsersRepository(IDbConnectionAdapter dbConnectionAdapter, IDateTime dateTime)
    {
        _dbConnectionAdapter = dbConnectionAdapter;
        _dateTime = dateTime;
    }

    public async Task<User?> CreateAsync(User user, CancellationToken cancellationToken)
    {
        user.CreatedDate = _dateTime.UtcNow;
        string sql = InsertUserQuery(user, user.CreatedDate);

        int rows = await _dbConnectionAdapter.ExecuteNonQueryAsync(sql, cancellationToken);

        if (rows <= 0)
        {
            return null;
        }

        return user;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var userFounded = await GetAsync(user.Id, cancellationToken);
        if (userFounded == null)
        {
            return false;
        }

        userFounded.Name = user.Name;
        userFounded.Email = user.Email;
        userFounded.PasswordHash = user.PasswordHash;

        var sql = UpdateUserQuery(userFounded);

        int rows = await _dbConnectionAdapter.ExecuteNonQueryAsync(sql, cancellationToken);

        return rows > 0;
    }

    public Task<User?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        string sql = SelectUserByIdQuery(id);

        return _dbConnectionAdapter.ExecuteReaderSingleAsync<User?>(sql, UserMapper.MapFromDbResult, cancellationToken);
    }

    public async Task<User?> GetByEmailAndPasswordAsync(string email, string passwordHash, CancellationToken cancellationToken)
    {
        string sql = SelectUserByEmailAndPasswordQuery(email, passwordHash);

        return await _dbConnectionAdapter.ExecuteReaderSingleAsync<User?>(sql, UserMapper.MapFromDbResult, cancellationToken);
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        string sql = SelectUserByEmailQuery(email);

        return _dbConnectionAdapter.ExecuteReaderSingleAsync<User?>(sql, UserMapper.MapFromDbResult, cancellationToken);
    }


    private static string InsertUserQuery(User user, DateTime createdDate)
    {
        return $"INSERT INTO users (id, name, email, passwordhash, createddate) VALUES('{user.Id}', '{user.Name}', '{user.Email}', '{user.PasswordHash}', '{createdDate}')";
    }

    private static string UpdateUserQuery(User userFounded)
    {
        return $"UPDATE users SET name = '{userFounded.Name}', email = '{userFounded.Email}', passwordhash = '{userFounded.PasswordHash}' WHERE id = '{userFounded.Id}'";
    }

    private static string SelectUserByIdQuery(Guid id)
    {
        return $"SELECT id, name, email, passwordhash FROM users WHERE id = '{id}'";
    }

    private static string SelectUserByEmailAndPasswordQuery(string email, string passwordHash)
    {
        return $"SELECT id, name, email, passwordhash FROM users WHERE email = '{email}' AND passwordhash = '{passwordHash}'";
    }

    private static string SelectUserByEmailQuery(string email)
    {
        return $"SELECT id, name, email, passwordhash FROM users WHERE email = '{email}'";
    }
}
