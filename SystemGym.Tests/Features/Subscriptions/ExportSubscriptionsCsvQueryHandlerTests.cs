namespace SystemGym.Tests.Features.Subscriptions;

using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using SystemGym.Application.Abstractions;
using SystemGym.Application.Features.Subscriptions.Queries;

/// <summary>
/// Tests para exportación CSV de suscripciones.
/// Valida que el CSV se genere correctamente con formato adecuado.
/// </summary>
public class ExportSubscriptionsCsvQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ExportSubscriptionsCsvQueryHandler>> _loggerMock;
    private readonly ExportSubscriptionsCsvQueryHandler _handler;

    public ExportSubscriptionsCsvQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ExportSubscriptionsCsvQueryHandler>>();
        _handler = new ExportSubscriptionsCsvQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SinParametrosDeBusqueda_DebeRetornarMensajeDeError()
    {
        // Arrange
        var query = new ExportSubscriptionsCsvQuery
        {
            TipoDocumento = null,
            NumeroDocumento = null,
            NombreCliente = null
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
        var csv = Encoding.UTF8.GetString(result);
        Assert.Contains("Sin criterio", csv);
    }

    [Fact]
    public async Task Handle_BusquedaPorDocumento_DebeRetornarCsvValido()
    {
        // Arrange
        var client = DomainFactory.CreateClient("CC", "12345678", "Juan Pérez");
        var plan = DomainFactory.CreatePlan("Plan Básico", 50000);
        var sub = DomainFactory.CreateSubscription(client, plan);

        var query = new ExportSubscriptionsCsvQuery
        {
            TipoDocumento = "CC",
            NumeroDocumento = "12345678"
        };

        _unitOfWorkMock
            .Setup(u => u.Clients.GetByNumeroDocumentoAsync("12345678", "CC", It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetAllByClientIdAsync(client.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { sub });

        _unitOfWorkMock
            .Setup(u => u.Plans.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);

        var csv = Encoding.UTF8.GetString(result);
        Assert.Contains("SubscriptionId", csv); // Header
        Assert.Contains("CC", csv); // Tipo documento
        Assert.Contains("12345678", csv); // Numero documento
        Assert.Contains("Juan Pérez", csv); // Nombre cliente
        Assert.Contains("Plan Básico", csv); // Descripción del plan
    }

    [Fact]
    public async Task Handle_CsvDebeTenerFormatoValido_ConComillasEnValoresConComas()
    {
        // Arrange
        var client = DomainFactory.CreateClient("TI", "1234567", "García, Juan M.");
        var plan = DomainFactory.CreatePlan("Plan Con, Coma");
        var sub = DomainFactory.CreateSubscription(client, plan);

        var query = new ExportSubscriptionsCsvQuery { NumeroDocumento = "1234567" };

        _unitOfWorkMock
            .Setup(u => u.Clients.GetByNumeroDocumentoAsync("1234567", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetAllByClientIdAsync(client.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { sub });

        _unitOfWorkMock
            .Setup(u => u.Plans.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        var csv = Encoding.UTF8.GetString(result);

        // Assert
        // Los valores con comas deben ir entre comillas
        Assert.Contains("\"García, Juan M.\"", csv);
        Assert.Contains("\"Plan Con, Coma\"", csv);
    }
}
