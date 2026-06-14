using Microsoft.Data.Sqlite;
using NominaApp.Models;
using NominaApp.Data.Sqlite;

namespace NominaApp.Services;

public class MovimientosService
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public MovimientosService(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> AgregarMovimientoAsync(MovimientoNomina movimiento, CancellationToken ct = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO MovimientosNomina (EmpleadoId, Periodo, ConceptoId, Monto)
VALUES ($EmpleadoId, $Periodo, $ConceptoId, $Monto);
SELECT last_insert_rowid();";

        cmd.Parameters.AddWithValue("$EmpleadoId", movimiento.EmpleadoId);
        cmd.Parameters.AddWithValue("$Periodo", movimiento.Periodo);
        cmd.Parameters.AddWithValue("$ConceptoId", movimiento.ConceptoId);
        cmd.Parameters.AddWithValue("$Monto", movimiento.Monto);

        var result = await cmd.ExecuteScalarAsync(ct);
        return result is null || result is DBNull ? 0 : Convert.ToInt64(result);
    }

    public async Task<List<(MovimientoNomina Movimiento, string ConceptoNombre, string ConceptoTipo)>> ListarMovimientosPorPeriodoAsync(string periodo, CancellationToken ct = default)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT
    m.Id,
    m.EmpleadoId,
    m.Periodo,
    m.ConceptoId,
    m.Monto,
    c.Nombre,
    c.Tipo
FROM MovimientosNomina m
INNER JOIN ConceptosNomina c ON c.Id = m.ConceptoId
WHERE m.Periodo = $Periodo
ORDER BY m.EmpleadoId, m.ConceptoId;";

        cmd.Parameters.AddWithValue("$Periodo", periodo);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<(MovimientoNomina, string, string)>();

        while (await reader.ReadAsync(ct))
        {
            var movimiento = new MovimientoNomina
            {
                Id = reader.GetInt64(0),
                EmpleadoId = reader.GetInt64(1),
                Periodo = reader.GetString(2),
                ConceptoId = reader.GetInt64(3),
                Monto = reader.GetDecimal(4)
            };
            var conceptoNombre = reader.GetString(5);
            var conceptoTipo = reader.GetString(6);

            items.Add((movimiento, conceptoNombre, conceptoTipo));
        }

        return items;
    }
}
