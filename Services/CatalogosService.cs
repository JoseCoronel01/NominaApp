using NominaApp.Models;
using NominaApp.Services.Queries;

namespace NominaApp.Services;

public class CatalogosService
{
    private readonly ISqlQueryExecutor _executor;

    public CatalogosService(ISqlQueryExecutor executor)
    {
        _executor = executor;
    }

    public Task<List<Empleado>> ListEmpleadosAsync(CancellationToken ct = default)
    {
        var query = new ListEmpleadosQuery();

        return _executor.QueryAsync(
            query,
            r => new Empleado
            {
                Id = r.GetInt64(0),
                Nombre = r.GetString(1),
                SalarioBase = r.GetDecimal(2),
                Activo = r.GetInt64(3) == 1
            },
            ct);
    }

    public Task<List<ConceptoNomina>> ListConceptosAsync(CancellationToken ct = default)
    {
        var query = new ListConceptosQuery();

        return _executor.QueryAsync(
            query,
            r => new ConceptoNomina
            {
                Id = r.GetInt64(0),
                Nombre = r.GetString(1),
                Tipo = r.GetString(2) == "Ingreso" ? ConceptoTipo.Ingreso : ConceptoTipo.Descuento,
                ValorFijo = r.IsDBNull(3) ? null : r.GetDecimal(3)
            },
            ct);
    }
}
