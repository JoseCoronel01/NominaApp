using Microsoft.Data.Sqlite;

namespace NominaApp.Services.Queries;

public interface ISqlQuery<T>
{
    string Sql { get; }
    void AddParameters(SqliteCommand command);
}

public abstract class SqlQueryBase<T> : ISqlQuery<T>
{
    public abstract string Sql { get; }

    public virtual void AddParameters(SqliteCommand command) { }
}
