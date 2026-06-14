using Microsoft.Data.Sqlite;

namespace NominaApp.Data.Sqlite;

public class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);

        var dbPath = Path.Combine(dataDir, "nominaapp.db");
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
    }

    public SqliteConnection CreateConnection() => new SqliteConnection(_connectionString);
}
