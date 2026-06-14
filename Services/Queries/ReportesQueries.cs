using Microsoft.Data.Sqlite;
using NominaApp.Models;

namespace NominaApp.Services.Queries;

public class GetDetallePagoPeriodoQuery : SqlQueryBase<DetallePagoPeriodo>
{
    private readonly string _periodo;

    public GetDetallePagoPeriodoQuery(string periodo)
    {
        _periodo = periodo;
    }

    public override string Sql => @"
        WITH Movs AS (
            SELECT
                m.EmpleadoId,
                m.Periodo,
                SUM(CASE WHEN c.Tipo = 'Ingreso' THEN m.Monto ELSE 0 END) AS TotalIngresos,
                SUM(CASE WHEN c.Tipo = 'Descuento' THEN m.Monto ELSE 0 END) AS TotalDescuentos
            FROM MovimientosNomina m
            INNER JOIN ConceptosNomina c ON c.Id = m.ConceptoId
            WHERE m.Periodo = $Periodo
            GROUP BY m.EmpleadoId, m.Periodo
        ),
        Abonos AS (
            SELECT
                p.EmpleadoId,
                strftime('%Y-%m', a.Fecha) AS Periodo,
                SUM(a.Monto) AS AbonoPrestamo
            FROM AbonosPrestamo a
            INNER JOIN Prestamos p ON p.Id = a.PrestamoId
            WHERE strftime('%Y-%m', a.Fecha) = $Periodo
            GROUP BY p.EmpleadoId, strftime('%Y-%m', a.Fecha)
        )
        SELECT
            e.Id AS EmpleadoId,
            $Periodo AS Periodo,
            COALESCE(m.TotalIngresos, 0) AS TotalIngresos,
            COALESCE(m.TotalDescuentos, 0) AS TotalDescuentos,
            COALESCE(ab.AbonoPrestamo, 0) AS AbonoPrestamo,
            (COALESCE(m.TotalIngresos, 0) - COALESCE(m.TotalDescuentos, 0) - COALESCE(ab.AbonoPrestamo, 0)) AS NetoAPagar
        FROM Empleados e
        LEFT JOIN Movs m ON m.EmpleadoId = e.Id AND m.Periodo = $Periodo
        LEFT JOIN Abonos ab ON ab.EmpleadoId = e.Id AND ab.Periodo = $Periodo
        WHERE e.Activo = 1
        ORDER BY e.Nombre;
        ";

    public override void AddParameters(SqliteCommand command)
    {
        command.Parameters.AddWithValue("$Periodo", _periodo);
    }
}
