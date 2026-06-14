using Microsoft.Data.Sqlite;

namespace NominaApp.Services.Queries;

public interface ISqlQueryExecutor
{
    Task<List<T>> QueryAsync<T>(ISqlQuery<T> query, Func<SqliteDataReader, T> map, CancellationToken ct = default);
    Task<T?> QuerySingleOrDefaultAsync<T>(ISqlQuery<T> query, Func<SqliteDataReader, T> map, CancellationToken ct = default);
}

public class SqlQueryExecutor : ISqlQueryExecutor
{
    private readonly Data.Sqlite.SqliteConnectionFactory _connectionFactory;

    public SqlQueryExecutor(Data.Sqlite.SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<T>> QueryAsync<T>(
        ISqlQuery<T> query,
        Func<SqliteDataReader, T> map,
        CancellationToken ct = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = query.Sql;
        query.AddParameters(cmd);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        var result = new List<T>();

        while (await reader.ReadAsync(ct))
        {
            result.Add(map(reader));
        }

        return result;
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(
        ISqlQuery<T> query,
        Func<SqliteDataReader, T> map,
        CancellationToken ct = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = query.Sql;
        query.AddParameters(cmd);

        using var reader = await cmd.ExecuteReaderAsync(ct);

        if (await reader.ReadAsync(ct))
            return map(reader);

        return default;
    }
}
