using Microsoft.Data.Sqlite;
using NominaApp.Data.Sqlite;
using NominaApp.Models;

namespace NominaApp.Services;

public class PrestamosService
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public PrestamosService(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> CrearPrestamoAsync(Prestamo prestamo, CancellationToken ct = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO Prestamos (EmpleadoId, PeriodoInicio, MontoInicial, Tasa, SaldoActual)
VALUES ($EmpleadoId, $PeriodoInicio, $MontoInicial, $Tasa, $SaldoActual);
SELECT last_insert_rowid();";

        var saldoActual = prestamo.SaldoActual; // si viene vacío, lo dejamos que el caller lo llene
        // Si quieres asegurar que arranque con MontoInicial:
        // saldoActual = prestamo.SaldoActual == 0 ? prestamo.MontoInicial : prestamo.SaldoActual;

        cmd.Parameters.AddWithValue("$EmpleadoId", prestamo.EmpleadoId);
        cmd.Parameters.AddWithValue("$PeriodoInicio", prestamo.PeriodoInicio);
        cmd.Parameters.AddWithValue("$MontoInicial", prestamo.MontoInicial);
        cmd.Parameters.AddWithValue("$Tasa", prestamo.Tasa);
        cmd.Parameters.AddWithValue("$SaldoActual", saldoActual);

        var result = await cmd.ExecuteScalarAsync(ct);
        return result is null || result is DBNull ? 0 : Convert.ToInt64(result);
    }

    public async Task<List<Prestamo>> ListarPrestamosPorEmpleadoAsync(long empleadoId, CancellationToken ct = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT Id, EmpleadoId, PeriodoInicio, MontoInicial, Tasa, SaldoActual
FROM Prestamos
WHERE EmpleadoId = $EmpleadoId
ORDER BY Id DESC;";

        cmd.Parameters.AddWithValue("$EmpleadoId", empleadoId);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<Prestamo>();

        while (await reader.ReadAsync(ct))
        {
            items.Add(new Prestamo
            {
                Id = reader.GetInt64(0),
                EmpleadoId = reader.GetInt64(1),
                PeriodoInicio = reader.GetString(2),
                MontoInicial = reader.GetDecimal(3),
                Tasa = reader.GetDecimal(4),
                SaldoActual = reader.GetDecimal(5)
            });
        }

        return items;
    }
}
