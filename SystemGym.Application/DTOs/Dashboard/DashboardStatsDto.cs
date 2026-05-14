namespace SystemGym.Application.DTOs.Dashboard;

/// <summary>
/// DTO con estadísticas del sistema para monitoreo.
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// Total de clientes registrados.
    /// </summary>
    public int TotalClientes { get; set; }

    /// <summary>
    /// Total de suscripciones registradas.
    /// </summary>
    public int TotalSuscripciones { get; set; }

    /// <summary>
    /// Número de suscripciones activas.
    /// </summary>
    public int SuscripcionesActivas { get; set; }

    /// <summary>
    /// Número de suscripciones expiradas.
    /// </summary>
    public int SuscripcionesExpiradas { get; set; }

    /// <summary>
    /// Ingresos totales (suma de todos los valores de suscripción).
    /// </summary>
    public decimal IngresosTotal { get; set; }

    /// <summary>
    /// Promedio de valor por suscripción.
    /// </summary>
    public decimal PromedioPorSuscripcion { get; set; }

    /// <summary>
    /// Plan más popular (nombre y cantidad de suscripciones).
    /// </summary>
    public PlanPopularDto? PlanMasPopular { get; set; }

    /// <summary>
    /// Distribución de suscripciones por tipo de período.
    /// </summary>
    public List<DistribucionPeriodoDto> DistribucionPorPeriodo { get; set; } = new();

    /// <summary>
    /// Fecha de generación de las estadísticas.
    /// </summary>
    public DateTime FechaGeneracion { get; set; }
}

/// <summary>
/// Información del plan más popular.
/// </summary>
public class PlanPopularDto
{
    public string Descripcion { get; set; } = string.Empty;
    public int CantidadSuscripciones { get; set; }
}

/// <summary>
/// Distribución de suscripciones por tipo de período.
/// </summary>
public class DistribucionPeriodoDto
{
    public string TipoPeriodo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}
