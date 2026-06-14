using Microsoft.Data.Sqlite;

namespace NominaApp.Services.Commands;

public class SqlCommandExecutor
{
    private readonly Data.Sqlite.SqliteConnectionFactory _connectionFactory;

    public SqlCommandExecutor(Data.Sqlite.SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> ExecuteAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ISqlCommand<long>
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = command.Sql;
        command.AddParameters(cmd);

        return await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<long> ExecuteScalarAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ISqlCommand<long>
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = command.Sql;
        command.AddParameters(cmd);

        var result = await cmd.ExecuteScalarAsync(ct);
        return result is null || result is DBNull ? 0 : Convert.ToInt64(result);
    }
}
