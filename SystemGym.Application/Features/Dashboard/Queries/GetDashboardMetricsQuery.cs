namespace SystemGym.Application.Features.Dashboard.Queries;

using SystemGym.Application.Abstractions;

/// <summary>
/// Query para obtener métricas del dashboard
/// </summary>
public class GetDashboardMetricsQuery : IQuery<DashboardMetricsDto>
{
}

/// <summary>
/// DTO con las métricas del dashboard
/// </summary>
public class DashboardMetricsDto
{
    /// <summary>Total de clientes registrados</summary>
    public int TotalClients { get; set; }

    /// <summary>Total de planes disponibles</summary>
    public int TotalPlans { get; set; }

    /// <summary>Total de suscripciones activas en el sistema</summary>
    public int TotalSubscriptions { get; set; }

    /// <summary>Suscripciones creadas este mes</summary>
    public int SubscriptionsThisMonth { get; set; }

    /// <summary>Ventas realizadas hoy</summary>
    public int SalesToday { get; set; }

    /// <summary>Monto total de ventas hoy</summary>
    public decimal SalesAmountToday { get; set; }

    /// <summary>Ventas realizadas este mes</summary>
    public int SalesThisMonth { get; set; }

    /// <summary>Monto total de ventas este mes</summary>
    public decimal SalesAmountThisMonth { get; set; }
}
