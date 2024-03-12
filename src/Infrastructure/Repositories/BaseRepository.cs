using Business.Models;
using Infrastructure.Adapters;

namespace Infrastructure.Repositories;

public abstract class BaseRepository<T> where T : BaseModel
{
    protected readonly IDbConnectionAdapter _dbConnectionAdapter;

    protected BaseRepository(IDbConnectionAdapter dbConnectionAdapter)
    {
        _dbConnectionAdapter = dbConnectionAdapter;
    }

    protected async Task<bool> ExecuteCommand(string sqlCommand, CancellationToken cancellationToken)
    {
        int rows = await _dbConnectionAdapter.ExecuteNonQueryAsync(sqlCommand, cancellationToken);

        return rows > 0;
    }

    protected Task<T?> ExecuteQuery(string sqlQuery, Func<Dictionary<string, object>, T> mapper, CancellationToken cancellationToken)
    {
        return _dbConnectionAdapter.ExecuteReaderSingleAsync<T?>(sqlQuery, mapper, cancellationToken);
    }

    protected Task<List<T>> ExecuteQueryList(string sqlQuery, Func<Dictionary<string, object>, T> mapper, CancellationToken cancellationToken)
    {
        return _dbConnectionAdapter.ExecuteReaderAsync<T>(sqlQuery, mapper, cancellationToken);
    }
}
