namespace Infrastructure.Adapters;

public interface IDbConnectionAdapter
{
    Task<int> ExecuteNonQueryAsync(string command, CancellationToken cancellationToken);
    Task<T?> ExecuteReaderSingleAsync<T>(string command, Func<Dictionary<string, object>, T> mapper, CancellationToken cancellationToken);
    Task<List<T>> ExecuteReaderAsync<T>(string command, Func<Dictionary<string, object>, T> mapper, CancellationToken cancellationToken);
}
