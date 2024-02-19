using Npgsql;

namespace Infrastructure.Adapters;

public class PostgresConnectionAdapter : IDbConnectionAdapter, IDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresConnectionAdapter(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<int> ExecuteNonQueryAsync(string command, CancellationToken cancellationToken)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var cmd = new NpgsqlCommand(command, conn);
        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<T?> ExecuteReaderSingleAsync<T>(string command, Func<Dictionary<string, object>, T> mapper, CancellationToken cancellationToken)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var cmd = new NpgsqlCommand(command, conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var data = GetRowReaderDictionaryResult(reader);
            return mapper(data);
        }

        return default;
    }

    public async Task<List<T>> ExecuteReaderAsync<T>(string command, Func<Dictionary<string, object>, T> mapper, CancellationToken cancellationToken)
    {
        var list = Array.Empty<T>().ToList();

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var cmd = new NpgsqlCommand(command, conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var data = GetRowReaderDictionaryResult(reader);
            list.Add(mapper(data));
        }

        return list;
    }

    private static Dictionary<string, object> GetRowReaderDictionaryResult(NpgsqlDataReader reader)
    {
        var data = new Dictionary<string, object>();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            data.Add(reader.GetName(i), reader.GetValue(i));
        }

        return data;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}