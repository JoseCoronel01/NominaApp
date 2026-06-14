using NominaApp.Models;
using NominaApp.Services.Queries;

namespace NominaApp.Services;

public class ReportesService
{
    private readonly ISqlQueryExecutor _executor;

    public ReportesService(ISqlQueryExecutor executor)
    {
        _executor = executor;
    }

    public Task<List<DetallePagoPeriodo>> GetDetallePagoPeriodoAsync(string periodo, CancellationToken ct = default)
    {
        var query = new GetDetallePagoPeriodoQuery(periodo);

        return _executor.QueryAsync(
            query,
            r => new DetallePagoPeriodo
            {
                EmpleadoId = r.GetInt64(0),
                Periodo = r.GetString(1),
                TotalIngresos = r.GetDecimal(2),
                TotalDescuentos = r.GetDecimal(3),
                AbonoPrestamo = r.GetDecimal(4),
                NetoAPagar = r.GetDecimal(5)
            },
            ct);
    }
}
