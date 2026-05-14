namespace SystemGym.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemGym.Application.Features.Dashboard.Queries;

/// <summary>
/// Controlador para endpoints de monitoreo y estadísticas del sistema.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : BaseController
{
    public DashboardController(ILogger<DashboardController> logger, IMediator mediator)
        : base(logger, mediator)
    {
    }

    /// <summary>
    /// Obtiene estadísticas del sistema: total de clientes, suscripciones,
    /// ingresos, planes populares y distribución por período.
    /// </summary>
    /// <remarks>
    /// Retorna métricas del sistema:
    /// - Cantidad total de clientes y suscripciones
    /// - Suscripciones activas y expiradas
    /// - Ingresos totales y promedio por suscripción
    /// - Plan más popular
    /// - Distribución de suscripciones por tipo de período (Dia/Mes)
    /// </remarks>
    /// <response code="200">Estadísticas obtenidas correctamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        try
        {
            var result = await Mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
            return OkResult(result, "Estadísticas obtenidas exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener estadísticas del dashboard");
            return InternalServerErrorResult("Error al obtener las estadísticas");
        }
    }

    /// <summary>
    /// Obtiene las métricas del dashboard (total clientes, planes, suscripciones, ventas)
    /// </summary>
    [HttpGet("metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMetrics(CancellationToken cancellationToken)
    {
        try
        {
            var result = await Mediator.Send(new GetDashboardMetricsQuery(), cancellationToken);
            return OkResult(result, "Métricas obtenidas exitosamente");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error al obtener métricas del dashboard");
            return InternalServerErrorResult("Error al obtener las métricas");
        }
    }
}
