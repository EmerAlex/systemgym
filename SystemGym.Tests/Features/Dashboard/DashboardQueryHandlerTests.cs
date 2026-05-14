namespace SystemGym.Tests.Features.Dashboard;

using Microsoft.Extensions.Logging;
using Moq;
using SystemGym.Application.Abstractions;
using SystemGym.Application.Features.Dashboard.Queries;

/// <summary>
/// Tests para GetDashboardStatsQueryHandler.
/// Valida generación de estadísticas del sistema.
/// </summary>
public class DashboardQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetDashboardStatsQueryHandler>> _loggerMock;
    private readonly GetDashboardStatsQueryHandler _handler;

    public DashboardQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetDashboardStatsQueryHandler>>();
        _handler = new GetDashboardStatsQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SinDatos_DebeRetornarCerosYNulls()
    {
        // Arrange
        _unitOfWorkMock
            .Setup(u => u.Clients.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SystemGym.Domain.Entities.Client>());

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SystemGym.Domain.Entities.Subscription>());

        _unitOfWorkMock
            .Setup(u => u.Plans.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SystemGym.Domain.Entities.Plan>());

        var query = new GetDashboardStatsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalClientes);
        Assert.Equal(0, result.TotalSuscripciones);
        Assert.Equal(0, result.SuscripcionesActivas);
        Assert.Equal(0, result.SuscripcionesExpiradas);
        Assert.Equal(0, result.IngresosTotal);
        Assert.Null(result.PlanMasPopular);
        Assert.Empty(result.DistribucionPorPeriodo);
    }

    [Fact]
    public async Task Handle_ConClientes_DebeContarTotalCorrectamente()
    {
        // Arrange
        var client1 = DomainFactory.CreateClient("CC", "11111111", "Juan Pérez");
        var client2 = DomainFactory.CreateClient("CC", "22222222", "María García");

        _unitOfWorkMock
            .Setup(u => u.Clients.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { client1, client2 });

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SystemGym.Domain.Entities.Subscription>());

        _unitOfWorkMock
            .Setup(u => u.Plans.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SystemGym.Domain.Entities.Plan>());

        var query = new GetDashboardStatsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalClientes);
        Assert.Equal(0, result.TotalSuscripciones);
    }

    [Fact]
    public async Task Handle_ConSuscripciones_DebeCalcularIngresosYPromedio()
    {
        // Arrange
        var client = DomainFactory.CreateClient("CC", "12345678", "Juan Pérez");
        var plan1 = DomainFactory.CreatePlan("Plan A", 50000);
        var plan2 = DomainFactory.CreatePlan("Plan B", 100000);

        var sub1 = DomainFactory.CreateSubscription(client, plan1);
        var sub2 = DomainFactory.CreateSubscription(client, plan2);

        _unitOfWorkMock
            .Setup(u => u.Clients.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { client });

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { sub1, sub2 });

        _unitOfWorkMock
            .Setup(u => u.Plans.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { plan1, plan2 });

        var query = new GetDashboardStatsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalSuscripciones);
        Assert.Equal(150000, result.IngresosTotal);
        Assert.Equal(75000, result.PromedioPorSuscripcion);
    }

    [Fact]
    public async Task Handle_PlanMasPopular_DebeRetornarPlanConMasSuscripciones()
    {
        // Arrange
        var client1 = DomainFactory.CreateClient("CC", "11111111", "Cliente 1");
        var client2 = DomainFactory.CreateClient("CC", "22222222", "Cliente 2");
        var client3 = DomainFactory.CreateClient("CC", "33333333", "Cliente 3");

        var planPopular = DomainFactory.CreatePlan("Plan Popular", 50000);
        var planMenosPopular = DomainFactory.CreatePlan("Plan Raro", 100000);

        // 3 suscripciones del plan popular
        var sub1 = DomainFactory.CreateSubscription(client1, planPopular);
        var sub2 = DomainFactory.CreateSubscription(client2, planPopular);
        var sub3 = DomainFactory.CreateSubscription(client3, planPopular);

        // 1 suscripción del plan menos popular
        var sub4 = DomainFactory.CreateSubscription(client1, planMenosPopular);

        _unitOfWorkMock
            .Setup(u => u.Clients.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { client1, client2, client3 });

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { sub1, sub2, sub3, sub4 });

        _unitOfWorkMock
            .Setup(u => u.Plans.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { planPopular, planMenosPopular });

        var query = new GetDashboardStatsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.PlanMasPopular);
        Assert.Equal("Plan Popular", result.PlanMasPopular.Descripcion);
        Assert.Equal(3, result.PlanMasPopular.CantidadSuscripciones);
    }

    [Fact]
    public async Task Handle_DistribucionPorPeriodo_DebeAgruparPorTipoPeriodo()
    {
        // Arrange
        var client1 = DomainFactory.CreateClient("CC", "11111111", "Cliente 1");
        var client2 = DomainFactory.CreateClient("CC", "22222222", "Cliente 2");
        var client3 = DomainFactory.CreateClient("CC", "33333333", "Cliente 3");

        var planMes = DomainFactory.CreatePlan("Plan Mensual", 50000, "Mes", 1);
        var planDia = DomainFactory.CreatePlan("Plan Diario", 5000, "Dia", 30);

        var sub1 = DomainFactory.CreateSubscription(client1, planMes);
        var sub2 = DomainFactory.CreateSubscription(client2, planMes);
        var sub3 = DomainFactory.CreateSubscription(client3, planDia);

        _unitOfWorkMock
            .Setup(u => u.Clients.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { client1, client2, client3 });

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { sub1, sub2, sub3 });

        _unitOfWorkMock
            .Setup(u => u.Plans.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { planMes, planDia });

        var query = new GetDashboardStatsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result.DistribucionPorPeriodo);
        var mes = result.DistribucionPorPeriodo.FirstOrDefault(d => d.TipoPeriodo == "Mes");
        var dia = result.DistribucionPorPeriodo.FirstOrDefault(d => d.TipoPeriodo == "Dia");
        
        Assert.NotNull(mes);
        Assert.NotNull(dia);
        Assert.Equal(2, mes.Cantidad);
        Assert.Equal(1, dia.Cantidad);
    }
}
