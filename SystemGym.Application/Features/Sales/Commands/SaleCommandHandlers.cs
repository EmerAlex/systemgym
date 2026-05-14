namespace SystemGym.Application.Features.Sales.Commands;

using MediatR;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Handler para el comando de crear venta
/// </summary>
public class CreateSaleCommandHandler : ICommandHandler<CreateSaleCommand, CreateSaleCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSaleCommandHandler> _logger;

    public CreateSaleCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateSaleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateSaleCommandResult> Handle(
        CreateSaleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validar que el cliente existe
            var client = await _unitOfWork.Clients
                .GetByIdAsync(request.ClientId, cancellationToken);

            if (client is null)
                return new CreateSaleCommandResult
                {
                    Success = false,
                    Message = "Cliente no existe",
                    Errors = new() { { "ClientId", new[] { "Cliente no encontrado" } } }
                };

            // Validar que el producto existe
            var product = await _unitOfWork.Products
                .GetByIdAsync(request.ProductId, cancellationToken);

            if (product is null)
                return new CreateSaleCommandResult
                {
                    Success = false,
                    Message = "Producto no existe",
                    Errors = new() { { "ProductId", new[] { "Producto no encontrado" } } }
                };

            // Validar stock disponible antes de crear la venta
            if (product.CantidadActual <= 0)
                return new CreateSaleCommandResult
                {
                    Success = false,
                    Message = "Sin stock disponible para este producto",
                    Errors = new() { { "ProductId", new[] { "Sin stock disponible" } } }
                };

            // Crear venta
            var sale = SalesHistory.Create(
                request.ClientId,
                request.ProductId,
                request.UserId,
                request.FechaVenta,
                product.Valor,
                request.Pagado,
                request.MedioPago,
                request.Referencia);

            // Descontar stock automáticamente
            product.DecrementStock();

            _unitOfWork.SalesHistory.Add(sale);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Venta creada: {SaleId}", sale.Id);

            return new CreateSaleCommandResult
            {
                Success = true,
                Message = "Venta creada exitosamente",
                Data = sale.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear venta");
            return new CreateSaleCommandResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}

/// <summary>
/// Handler para el comando de marcar venta como pagada
/// </summary>
public class MarkSaleAsPaidCommandHandler : ICommandHandler<MarkSaleAsPaidCommand, MarkSaleAsPaidCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkSaleAsPaidCommandHandler> _logger;

    public MarkSaleAsPaidCommandHandler(IUnitOfWork unitOfWork, ILogger<MarkSaleAsPaidCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<MarkSaleAsPaidCommandResult> Handle(
        MarkSaleAsPaidCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var sale = await _unitOfWork.SalesHistory
                .GetByIdAsync(request.SaleId, cancellationToken);

            if (sale is null)
                return new MarkSaleAsPaidCommandResult
                {
                    Success = false,
                    Message = "Venta no encontrada"
                };

            if (sale.Pagado)
                return new MarkSaleAsPaidCommandResult
                {
                    Success = false,
                    Message = "La venta ya fue marcada como pagada"
                };

            // Marcar como pagada
            sale.MarkAsPaid(request.MedioPago, request.Referencia);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Venta marcada como pagada: {SaleId}", sale.Id);

            return new MarkSaleAsPaidCommandResult
            {
                Success = true,
                Message = "Venta marcada como pagada exitosamente",
                Data = sale.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar venta como pagada");
            return new MarkSaleAsPaidCommandResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}
