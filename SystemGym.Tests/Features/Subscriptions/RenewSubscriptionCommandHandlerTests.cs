namespace SystemGym.Tests.Features.Subscriptions;

using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SystemGym.Application.Abstractions;
using SystemGym.Application.Features.Subscriptions.Commands;
using SystemGym.Domain.Entities;

/// <summary>
/// Tests para el flujo de renovación de suscripciones.
/// Valida que la renovación resetea ingresos, limpia UltimoIngreso y activa la poliza.
/// </summary>
public class RenewSubscriptionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RenewSubscriptionCommandHandler _handler;

    public RenewSubscriptionCommandHandlerTests()
    {
        _uowMock       = new Mock<IUnitOfWork>();
        _mediatorMock  = new Mock<IMediator>();

        _handler = new RenewSubscriptionCommandHandler(
            _uowMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task Renew_PlanMensual_ResetearIngresoA30YActivar()
    {
        // Arrange — plan mensual 1 mes = 30 ingresos
        var plan = DomainFactory.CreatePlan("Plan Mensual", 80000, tipoPeriodo: "Mes", cantidadIntervalosPeriodo: 1);
        var client = DomainFactory.CreateClient();

        // Crear suscripción con fechas válidas (hoy), simula que todos los ingresos fueron consumidos
        var subscription = Subscription.Create(
            client.Id,
            plan.Id,
            DateTime.Today,
            tieneExpiracion: true,
            DateTime.Today.AddMonths(1),
            plan.Valor,
            cantidadIngresos: 0); // Todos los ingresos consumidos

        _uowMock.Setup(u => u.Subscriptions.GetByIdAsync(subscription.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subscription);
        _uowMock.Setup(u => u.Plans.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(plan);
        _uowMock.Setup(u => u.Subscriptions.Update(It.IsAny<Subscription>()));
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new RenewSubscriptionCommand
        {
            SubscriptionId = subscription.Id,
            NuevoInicio    = DateTime.Today,
            TieneExpiracion = true
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(subscription.Id, result.Data);

        // Los 30 ingresos deben haberse restaurado (reset, no increment)
        Assert.Equal(30, subscription.CantidadIngresos);

        // UltimoIngreso debe ser null — no ha ingresado aún en el nuevo periodo
        Assert.Null(subscription.UltimoIngreso);

        // La poliza debe estar activa tras la renovación
        Assert.True(subscription.Activa);

        // Las nuevas fechas deben ser las esperadas
        Assert.Equal(DateTime.Today, subscription.InicioVigencia.Date);
        Assert.Equal(DateTime.Today.AddMonths(1), subscription.FinVigencia.Date);
    }

    [Fact]
    public async Task Renew_PlanDiario5Dias_ResetearIngresoA5()
    {
        // Arrange — plan diario 5 días = 5 ingresos
        var plan = DomainFactory.CreatePlan("Plan 5 Días", 50000, tipoPeriodo: "Dia", cantidadIntervalosPeriodo: 5);
        var client = DomainFactory.CreateClient();

        var subscription = Subscription.Create(
            client.Id,
            plan.Id,
            DateTime.Today,
            tieneExpiracion: true,
            DateTime.Today.AddDays(5),
            plan.Valor,
            cantidadIngresos: 0); // Todos los ingresos consumidos

        _uowMock.Setup(u => u.Subscriptions.GetByIdAsync(subscription.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subscription);
        _uowMock.Setup(u => u.Plans.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(plan);
        _uowMock.Setup(u => u.Subscriptions.Update(It.IsAny<Subscription>()));
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new RenewSubscriptionCommand
        {
            SubscriptionId  = subscription.Id,
            NuevoInicio     = DateTime.Today,
            TieneExpiracion = true
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success, result.Message);
        Assert.Equal(5, subscription.CantidadIngresos); // 5 ingresos del plan diario
        Assert.Null(subscription.UltimoIngreso);
        Assert.True(subscription.Activa);
    }

    [Fact]
    public async Task Renew_SuscripcionInexistente_DebeRetornarError()
    {
        // Arrange
        _uowMock.Setup(u => u.Subscriptions.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Subscription?)null);

        var command = new RenewSubscriptionCommand
        {
            SubscriptionId  = Guid.NewGuid(),
            NuevoInicio     = DateTime.Today,
            TieneExpiracion = true
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.Contains("SubscriptionId", result.Errors.Keys);
    }

    [Fact]
    public async Task Renew_GeneraEventoDeDominio_QueCreaSaleAutomatica()
    {
        // Arrange
        var plan   = DomainFactory.CreatePlan("Plan Mensual", 80000);
        var client = DomainFactory.CreateClient();
        var subscription = DomainFactory.CreateSubscription(client, plan); // inicio = DateTime.Today

        _uowMock.Setup(u => u.Subscriptions.GetByIdAsync(subscription.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subscription);
        _uowMock.Setup(u => u.Plans.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(plan);
        _uowMock.Setup(u => u.Subscriptions.Update(It.IsAny<Subscription>()));
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new RenewSubscriptionCommand
        {
            SubscriptionId  = subscription.Id,
            NuevoInicio     = DateTime.Today,
            TieneExpiracion = true
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success, result.Message);

        // Debe publicar evento de dominio (SubscriptionRenewedDomainEvent)
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }
}
