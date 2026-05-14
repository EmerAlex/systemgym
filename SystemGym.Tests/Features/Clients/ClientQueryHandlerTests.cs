namespace SystemGym.Tests.Features.Clients;

using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using SystemGym.Application.Abstractions;
using SystemGym.Application.Features.Clients.Queries;
using SystemGym.Application.Mappings;

/// <summary>
/// Tests para GetClientQueryHandler.
/// Valida obtención de cliente por ID.
/// </summary>
public class GetClientQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetClientQueryHandler>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly GetClientQueryHandler _handler;

    public GetClientQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetClientQueryHandler>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ClientMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetClientQueryHandler(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ClienteExistente_DebeRetornarClienteDto()
    {
        // Arrange
        var client = DomainFactory.CreateClient("CC", "12345678", "Juan Pérez", "3101234567");
        var query = new GetClientQuery { ClientId = client.Id };

        _unitOfWorkMock
            .Setup(u => u.Clients.GetByIdAsync(client.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(client.NombreCompleto, result.NombreCompleto);
        Assert.Equal(client.NumeroDocumento, result.NumeroDocumento);
        Assert.Equal("CC", result.TipoDocumento);
    }

    [Fact]
    public async Task Handle_ClienteNoExistente_DebeRetornarNull()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var query = new GetClientQuery { ClientId = clientId };

        _unitOfWorkMock
            .Setup(u => u.Clients.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SystemGym.Domain.Entities.Client?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}

/// <summary>
/// Tests para GetClientsQueryHandler.
/// Valida obtención de lista paginada de clientes.
/// </summary>
public class GetClientsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetClientsQueryHandler>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly GetClientsQueryHandler _handler;

    public GetClientsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetClientsQueryHandler>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ClientMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetClientsQueryHandler(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ConClientes_DebeRetornarListaPaginada()
    {
        // Arrange
        var client1 = DomainFactory.CreateClient("CC", "11111111", "Juan Pérez");
        var client2 = DomainFactory.CreateClient("CC", "22222222", "María García");

        var query = new GetClientsQuery { PageNumber = 1, PageSize = 10 };

        _unitOfWorkMock
            .Setup(u => u.Clients.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { client1, client2 }, 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task Handle_SinClientes_DebeRetornarListaVacia()
    {
        // Arrange
        var query = new GetClientsQuery { PageNumber = 1, PageSize = 10 };

        _unitOfWorkMock
            .Setup(u => u.Clients.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new SystemGym.Domain.Entities.Client[] { }, 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Data.Count);
        Assert.Empty(result.Data);
    }
}
