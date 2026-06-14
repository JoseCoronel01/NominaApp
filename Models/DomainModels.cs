namespace NominaApp.Models;

public class Empleado
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal SalarioBase { get; set; }
    public bool Activo { get; set; } = true;
}

public enum ConceptoTipo
{
    Ingreso = 1,
    Descuento = 2
}

public class ConceptoNomina
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ConceptoTipo Tipo { get; set; }
    public decimal? ValorFijo { get; set; }
}

public class MovimientoNomina
{
    public long Id { get; set; }
    public long EmpleadoId { get; set; }
    public string Periodo { get; set; } = string.Empty; // YYYY-MM
    public long ConceptoId { get; set; }
    public decimal Monto { get; set; }
}

public class Prestamo
{
    public long Id { get; set; }
    public long EmpleadoId { get; set; }
    public string PeriodoInicio { get; set; } = string.Empty; // YYYY-MM
    public decimal MontoInicial { get; set; }
    public decimal Tasa { get; set; }
    public decimal SaldoActual { get; set; }
}

public class AbonoPrestamo
{
    public long Id { get; set; }
    public long PrestamoId { get; set; }
    public DateOnly Fecha { get; set; }
    public decimal Monto { get; set; }
}

public class DetallePagoPeriodo
{
    public long EmpleadoId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public decimal TotalIngresos { get; set; }
    public decimal TotalDescuentos { get; set; }
    public decimal AbonoPrestamo { get; set; }
    public decimal NetoAPagar { get; set; }
}
