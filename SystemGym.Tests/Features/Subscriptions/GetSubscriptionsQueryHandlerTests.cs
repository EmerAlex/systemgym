namespace SystemGym.Tests.Features.Subscriptions;

using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using SystemGym.Application.Abstractions;
using SystemGym.Application.DTOs.Subscriptions;
using SystemGym.Application.Features.Subscriptions.Queries;
using SystemGym.Application.Mappings;

/// <summary>
/// Tests para GetSubscriptionsQueryHandler.
/// Valida búsqueda por documento, búsqueda por nombre, y enriquecimiento de datos.
/// </summary>
public class GetSubscriptionsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetSubscriptionsQueryHandler>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly GetSubscriptionsQueryHandler _handler;

    public GetSubscriptionsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetSubscriptionsQueryHandler>>();

        // Configurar AutoMapper con los profiles reales
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SubscriptionMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetSubscriptionsQueryHandler(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SinParametrosDeBusqueda_DebeRetornarListaVacia()
    {
        // Arrange
        var query = new GetSubscriptionsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            TipoDocumento = null,
            NumeroDocumento = null,
            NombreCliente = null
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ClienteEncontrado);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task Handle_ClienteNoEncontradoPorDocumento_DebeRetornar404()
    {
        // Arrange
        var query = new GetSubscriptionsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            TipoDocumento = "CC",
            NumeroDocumento = "99999999",
            NombreCliente = null
        };

        _unitOfWorkMock
            .Setup(u => u.Clients.GetByNumeroDocumentoAsync("99999999", "CC", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SystemGym.Domain.Entities.Client?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.ClienteEncontrado);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_ClienteEncontradoPorDocumento_DebeRetornarSuscripciones()
    {
        // Arrange
        var client = DomainFactory.CreateClient("CC", "12345678", "Juan Pérez");
        var plan = DomainFactory.CreatePlan("Plan Básico", 50000);
        var sub = DomainFactory.CreateSubscription(client, plan);

        var query = new GetSubscriptionsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            TipoDocumento = "CC",
            NumeroDocumento = "12345678",
            NombreCliente = null
        };

        _unitOfWorkMock
            .Setup(u => u.Clients.GetByNumeroDocumentoAsync("12345678", "CC", It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetPagedByClientAsync(
                client.Id, 1, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { sub }, 1));

        _unitOfWorkMock
            .Setup(u => u.Plans.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ClienteEncontrado);
        Assert.NotEmpty(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("Juan Pérez", result.ClienteNombreCompleto);
        Assert.Equal("CC", result.Data.First().ClientTipoDocumento);
        Assert.Equal("12345678", result.Data.First().ClientNumeroDocumento);
    }

    [Fact]
    public async Task Handle_BusquedaPorNombreConUnResultado_DebeRetornarSuscripciones()
    {
        // Arrange
        var client = DomainFactory.CreateClient("CC", "12345678", "Juan Pérez García");
        var plan = DomainFactory.CreatePlan("Plan Pro", 100000);
        var sub = DomainFactory.CreateSubscription(client, plan);

        var query = new GetSubscriptionsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            TipoDocumento = null,
            NumeroDocumento = null,
            NombreCliente = "Juan"
        };

        _unitOfWorkMock
            .Setup(u => u.Clients.SearchByNombreAsync("Juan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { client });

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetPagedByClientAsync(
                client.Id, 1, 100, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { sub }, 1));

        _unitOfWorkMock
            .Setup(u => u.Plans.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ClienteEncontrado);
        Assert.NotEmpty(result.Data);
        Assert.Equal("Juan Pérez García", result.ClienteNombreCompleto);
    }

    [Fact]
    public async Task Handle_BusquedaPorNombreConVariosResultados_DebeAgregarSuscripcionesDeTodasEllos()
    {
        // Arrange
        var client1 = DomainFactory.CreateClient("CC", "11111111", "Juan Pérez");
        var client2 = DomainFactory.CreateClient("CC", "22222222", "Juan García");

        var plan = DomainFactory.CreatePlan("Plan Básico");
        var sub1 = DomainFactory.CreateSubscription(client1, plan);
        var sub2 = DomainFactory.CreateSubscription(client2, plan);

        var query = new GetSubscriptionsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            TipoDocumento = null,
            NumeroDocumento = null,
            NombreCliente = "Juan"
        };

        _unitOfWorkMock
            .Setup(u => u.Clients.SearchByNombreAsync("Juan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { client1, client2 });

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetPagedByClientAsync(
                client1.Id, 1, 100, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { sub1 }, 1));

        _unitOfWorkMock
            .Setup(u => u.Subscriptions.GetPagedByClientAsync(
                client2.Id, 1, 100, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { sub2 }, 1));

        _unitOfWorkMock
            .Setup(u => u.Plans.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ClienteEncontrado);
        Assert.Equal(2, result.Data.Count);
        Assert.Null(result.ClienteNombreCompleto); // No tiene un único cliente
    }

    [Fact]
    public async Task Handle_BusquedaPorNombreSinResultados_DebeRetornar404()
    {
        // Arrange
        var query = new GetSubscriptionsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            NombreCliente = "NoExiste"
        };

        _unitOfWorkMock
            .Setup(u => u.Clients.SearchByNombreAsync("NoExiste", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SystemGym.Domain.Entities.Client[] { });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.ClienteEncontrado);
        Assert.Empty(result.Data);
    }
}
