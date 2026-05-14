namespace SystemGym.Application.Features.Dashboard.Queries;

using SystemGym.Application.Abstractions;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler para GetDashboardMetricsQuery
/// </summary>
public class GetDashboardMetricsQueryHandler : IQueryHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetDashboardMetricsQueryHandler> _logger;

    public GetDashboardMetricsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetDashboardMetricsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DashboardMetricsDto> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Total clientes
            var allClients = await _unitOfWork.Clients.GetAllAsync(cancellationToken);
            var totalClients = allClients.Count();

            // Total planes
            var allPlans = await _unitOfWork.Plans.GetAllAsync(cancellationToken);
            var totalPlans = allPlans.Count();

            // Total suscripciones
            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync(cancellationToken);
            var totalSubscriptions = allSubscriptions.Count();

            // Suscripciones este mes
            var subscriptionsThisMonth = allSubscriptions
                .Where(s => s.CreatedAt.Date >= firstDayOfMonth && s.CreatedAt.Date <= lastDayOfMonth)
                .Count();

            // Ventas hoy
            var allSales = await _unitOfWork.SalesHistory.GetAllAsync(cancellationToken);
            var salesToday = allSales
                .Where(s => s.FechaVenta.Date == today)
                .Count();
            var salesAmountToday = allSales
                .Where(s => s.FechaVenta.Date == today)
                .Sum(s => s.Monto);

            // Ventas este mes
            var salesThisMonth = allSales
                .Where(s => s.FechaVenta.Date >= firstDayOfMonth && s.FechaVenta.Date <= lastDayOfMonth)
                .Count();
            var salesAmountThisMonth = allSales
                .Where(s => s.FechaVenta.Date >= firstDayOfMonth && s.FechaVenta.Date <= lastDayOfMonth)
                .Sum(s => s.Monto);

            _logger.LogInformation(
                "Dashboard metrics retrieved: {TotalClients} clients, {TotalPlans} plans, {TotalSubscriptions} subscriptions, {SalesToday} sales today (${SalesAmountToday})",
                totalClients, totalPlans, totalSubscriptions, salesToday, salesAmountToday);

            return new DashboardMetricsDto
            {
                TotalClients = totalClients,
                TotalPlans = totalPlans,
                TotalSubscriptions = totalSubscriptions,
                SubscriptionsThisMonth = subscriptionsThisMonth,
                SalesToday = salesToday,
                SalesAmountToday = salesAmountToday,
                SalesThisMonth = salesThisMonth,
                SalesAmountThisMonth = salesAmountThisMonth,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard metrics");
            throw;
        }
    }
}
