namespace SystemGym.Application.Features.Products.Commands;

using MediatR;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Handler para el comando de crear producto
/// </summary>
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateProductCommandResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = Product.Create(request.Descripcion, request.Valor);

            _unitOfWork.Products.Add(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Producto creado: {ProductId}", product.Id);

            return new CreateProductCommandResult
            {
                Success = true,
                Message = "Producto creado exitosamente",
                Data = product.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return new CreateProductCommandResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}
