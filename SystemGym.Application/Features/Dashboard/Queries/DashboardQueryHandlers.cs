namespace SystemGym.Application.Features.Dashboard.Queries;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Dashboard;
using SystemGym.Domain.ValueObjects;

/// <summary>
/// Handler para GetDashboardStatsQuery.
/// Genera estadísticas del sistema para monitoreo.
/// </summary>
public class GetDashboardStatsQueryHandler : IQueryHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetDashboardStatsQueryHandler> _logger;

    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetDashboardStatsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Generando estadísticas del dashboard");

            // Obtener todos los clientes y suscripciones
            var clients = (await _unitOfWork.Clients.GetAllAsync(cancellationToken)).ToList();
            var subscriptions = (await _unitOfWork.Subscriptions.GetAllAsync(cancellationToken)).ToList();
            var plans = (await _unitOfWork.Plans.GetAllAsync(cancellationToken)).ToList();

            var totalClientes = clients.Count;
            var totalSuscripciones = subscriptions.Count;
            var ahora = DateTime.UtcNow;

            // Contar suscripciones activas (inicio <= ahora)
            var subscripcionesActivas = subscriptions
                .Count(s => s.InicioVigencia <= ahora && (s.FinVigencia == DateTime.MaxValue || s.FinVigencia > ahora));

            // Contar suscripciones expiradas (fin < ahora)
            var subscripcionesExpiradas = subscriptions
                .Count(s => s.TieneExpiracion && s.FinVigencia < ahora);

            // Calcular ingresos totales (suma de valores)
            var ingresosTotal = subscriptions.Sum(s => s.Valor);
            var promedioPorSuscripcion = totalSuscripciones > 0 ? ingresosTotal / totalSuscripciones : 0;

            // Plan más popular
            var planMasPopular = CalcularPlanMasPopular(subscriptions, plans);

            // Distribución por período
            var distribucionPeriodo = CalcularDistribucionPeriodo(subscriptions, plans);

            return new DashboardStatsDto
            {
                TotalClientes = totalClientes,
                TotalSuscripciones = totalSuscripciones,
                SuscripcionesActivas = subscripcionesActivas,
                SuscripcionesExpiradas = subscripcionesExpiradas,
                IngresosTotal = ingresosTotal,
                PromedioPorSuscripcion = promedioPorSuscripcion,
                PlanMasPopular = planMasPopular,
                DistribucionPorPeriodo = distribucionPeriodo,
                FechaGeneracion = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar estadísticas del dashboard");
            throw;
        }
    }

    private PlanPopularDto? CalcularPlanMasPopular(
        List<SystemGym.Domain.Entities.Subscription> subscriptions,
        List<SystemGym.Domain.Entities.Plan> plans)
    {
        if (!subscriptions.Any())
            return null;

        var subscripcionesPorPlan = subscriptions
            .GroupBy(s => s.PlanId)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (subscripcionesPorPlan is null)
            return null;

        var plan = plans.FirstOrDefault(p => p.Id == subscripcionesPorPlan.Key);
        if (plan is null)
            return null;

        return new PlanPopularDto
        {
            Descripcion = plan.Descripcion,
            CantidadSuscripciones = subscripcionesPorPlan.Count()
        };
    }

    private List<DistribucionPeriodoDto> CalcularDistribucionPeriodo(
        List<SystemGym.Domain.Entities.Subscription> subscriptions,
        List<SystemGym.Domain.Entities.Plan> plans)
    {
        return subscriptions
            .GroupBy(s => plans.FirstOrDefault(p => p.Id == s.PlanId)?.TipoPeriodo.Value ?? "Desconocido")
            .OrderBy(g => g.Key)
            .Select(g => new DistribucionPeriodoDto
            {
                TipoPeriodo = g.Key,
                Cantidad = g.Count()
            })
            .ToList();
    }
}
