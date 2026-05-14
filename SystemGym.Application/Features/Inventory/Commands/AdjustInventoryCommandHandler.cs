namespace SystemGym.Application.Features.Inventory.Commands;

using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Handler para ajustar inventario de un producto y registrar el log append-only.
/// </summary>
public class AdjustInventoryCommandHandler : ICommandHandler<AdjustInventoryCommand, AdjustInventoryCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public AdjustInventoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AdjustInventoryCommandResult> Handle(
        AdjustInventoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);

            if (product is null)
            {
                return new AdjustInventoryCommandResult
                {
                    Success = false,
                    Message = "El producto no existe",
                    Errors = new() { { "ProductId", ["Producto no encontrado"] } }
                };
            }

            if (request.Cambio == 0)
            {
                return new AdjustInventoryCommandResult
                {
                    Success = false,
                    Message = "El cambio de inventario no puede ser cero",
                    Errors = new() { { "Cambio", ["El cambio debe ser diferente de cero"] } }
                };
            }

            var cantidadAnterior = product.CantidadActual;
            var cantidadNueva = cantidadAnterior + request.Cambio;

            if (cantidadNueva < 0)
            {
                return new AdjustInventoryCommandResult
                {
                    Success = false,
                    Message = "La cantidad resultante no puede ser negativa",
                    Errors = new() { { "Cambio", ["Inventario insuficiente para aplicar el ajuste"] } }
                };
            }

            var operacion = request.Cambio > 0 ? "Add" : "Reduce";

            product.UpdateInventory(cantidadNueva);

            var inventoryLog = InventoryLog.Create(
                request.ProductId,
                request.UserId,
                cantidadAnterior,
                cantidadNueva,
                operacion,
                request.MotivoAuditoria);

            _unitOfWork.Products.Update(product);
            _unitOfWork.InventoryLogs.Add(inventoryLog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AdjustInventoryCommandResult
            {
                Success = true,
                Message = "Inventario ajustado exitosamente",
                ProductId = product.Id,
                LogId = inventoryLog.Id,
                CantidadAnterior = cantidadAnterior,
                CantidadNueva = cantidadNueva,
                Diferencia = inventoryLog.Diferencia,
                Operacion = inventoryLog.Operacion
            };
        }
        catch (Exception ex)
        {
            return new AdjustInventoryCommandResult
            {
                Success = false,
                Message = $"Error al ajustar el inventario: {ex.Message}"
            };
        }
    }
}
