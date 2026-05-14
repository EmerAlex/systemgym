namespace SystemGym.Tests.Features.Subscriptions;

using Microsoft.Extensions.Logging;
using Moq;
using SystemGym.Application.Abstractions;
using SystemGym.Application.Features.Plans.Queries;
using SystemGym.Application.Mappings;
using AutoMapper;

/// <summary>
/// Tests para handlers de planes.
/// Valida obtención de planes.
/// </summary>
public class GetPlansQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetPlansQueryHandler>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly GetPlansQueryHandler _handler;

    public GetPlansQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetPlansQueryHandler>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PlanMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetPlansQueryHandler(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ConPlanes_DebeRetornarListaPaginada()
    {
        // Arrange
        var plan1 = DomainFactory.CreatePlan("Plan A", 50000);
        var plan2 = DomainFactory.CreatePlan("Plan B", 100000);

        var query = new GetPlansQuery { PageNumber = 1, PageSize = 10 };

        _unitOfWorkMock
            .Setup(u => u.Plans.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { plan1, plan2 }, 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task Handle_SinPlanes_DebeRetornarListaVacia()
    {
        // Arrange
        var query = new GetPlansQuery { PageNumber = 1, PageSize = 10 };

        _unitOfWorkMock
            .Setup(u => u.Plans.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new SystemGym.Domain.Entities.Plan[] { }, 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalCount);
    }
}
