using Microsoft.Data.Sqlite;

namespace NominaApp.Data.Sqlite;

public class SqliteDbInitializer
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteDbInitializer(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Initialize()
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Empleados (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                SalarioBase REAL NOT NULL,
                Activo INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS ConceptosNomina (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Tipo TEXT NOT NULL CHECK (Tipo IN ('Ingreso','Descuento')),
                ValorFijo REAL NULL
            );

            CREATE TABLE IF NOT EXISTS MovimientosNomina (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EmpleadoId INTEGER NOT NULL,
                Periodo TEXT NOT NULL, -- YYYY-MM
                ConceptoId INTEGER NOT NULL,
                Monto REAL NOT NULL,
                FOREIGN KEY(EmpleadoId) REFERENCES Empleados(Id),
                FOREIGN KEY(ConceptoId) REFERENCES ConceptosNomina(Id)
            );

            CREATE TABLE IF NOT EXISTS Prestamos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EmpleadoId INTEGER NOT NULL,
                PeriodoInicio TEXT NOT NULL,
                MontoInicial REAL NOT NULL,
                Tasa REAL NOT NULL,
                SaldoActual REAL NOT NULL,
                FOREIGN KEY(EmpleadoId) REFERENCES Empleados(Id)
            );

            CREATE TABLE IF NOT EXISTS AbonosPrestamo (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PrestamoId INTEGER NOT NULL,
                Fecha TEXT NOT NULL,
                Monto REAL NOT NULL,
                FOREIGN KEY(PrestamoId) REFERENCES Prestamos(Id)
            );

            -- Seed opcional
            INSERT OR IGNORE INTO Empleados (Id, Nombre, SalarioBase, Activo)
            VALUES (1, 'Empleado Demo', 1000, 1);
            ";

        cmd.ExecuteNonQuery();
    }
}
