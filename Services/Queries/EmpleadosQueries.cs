using Microsoft.Data.Sqlite;
using NominaApp.Models;

namespace NominaApp.Services.Queries;

public class ListEmpleadosQuery : SqlQueryBase<Empleado>
{
    public override string Sql => @"
        SELECT Id, Nombre, SalarioBase, Activo
        FROM Empleados
        ORDER BY Nombre;";

    public override void AddParameters(SqliteCommand command) { }
    public ListEmpleadosQuery() { }
}

public class GetEmpleadoByIdQuery : SqlQueryBase<Empleado>
{
    private readonly long _id;

    public GetEmpleadoByIdQuery(long id) => _id = id;

    public override string Sql => @"
        SELECT Id, Nombre, SalarioBase, Activo
        FROM Empleados
        WHERE Id = $Id;";

    public override void AddParameters(SqliteCommand command)
    {
        command.Parameters.AddWithValue("$Id", _id);
    }
}
