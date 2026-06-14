using NominaApp.Models;

namespace NominaApp.Services.Queries;

public class ListConceptosQuery : SqlQueryBase<ConceptoNomina>
{
    public override string Sql => @"
        SELECT Id, Nombre, Tipo, ValorFijo
        FROM ConceptosNomina
        ORDER BY Id;";
}
