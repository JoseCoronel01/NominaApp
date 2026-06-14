using Microsoft.Data.Sqlite;

namespace NominaApp.Services.Commands;

public interface ISqlCommand<T>
{
    string Sql { get; }
    void AddParameters(SqliteCommand command);
}

public abstract class SqlCommandBase<T> : ISqlCommand<T>
{
    public abstract string Sql { get; }
    public abstract void AddParameters(SqliteCommand command);
}
