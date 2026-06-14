using Microsoft.Data.Sqlite;
using NominaApp.Data.Sqlite;
using NominaApp.Models;

namespace NominaApp.Services;

public class AbonosService
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public AbonosService(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> RegistrarAbonoAsync(AbonoPrestamo abono, CancellationToken ct = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var tran = conn.BeginTransaction();

        try
        {
            // Insert abono
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"
INSERT INTO AbonosPrestamo (PrestamoId, Fecha, Monto)
VALUES ($PrestamoId, $Fecha, $Monto);
SELECT last_insert_rowid();";

                cmd.Parameters.AddWithValue("$PrestamoId", abono.PrestamoId);
                cmd.Parameters.AddWithValue("$Fecha", abono.Fecha.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("$Monto", abono.Monto);

                var id = await cmd.ExecuteScalarAsync(ct);
                var abonoId = id is null || id is DBNull ? 0 : Convert.ToInt64(id);

                // Update saldo
                using (var cmd2 = conn.CreateCommand())
                {
                    cmd2.Transaction = tran;
                    cmd2.CommandText = @"
UPDATE Prestamos
SET SaldoActual = 
    CASE 
        WHEN SaldoActual - $Monto < 0 THEN 0
        ELSE SaldoActual - $Monto
    END
WHERE Id = $PrestamoId;";

                    cmd2.Parameters.AddWithValue("$PrestamoId", abono.PrestamoId);
                    cmd2.Parameters.AddWithValue("$Monto", abono.Monto);

                    await cmd2.ExecuteNonQueryAsync(ct);
                }

                await tran.CommitAsync(ct);
                return abonoId;
            }
        }
        catch
        {
            await tran.RollbackAsync(ct);
            throw;
        }
    }
}
