namespace SystemGym.Application.Features.Sales.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para crear una venta
/// </summary>
public class CreateSaleCommand : ICommand<CreateSaleCommandResult>
{
    public required Guid ClientId { get; set; }
    public required Guid ProductId { get; set; }
    public required Guid UserId { get; set; } // Usuario que realiza la venta
    public required DateTime FechaVenta { get; set; }
    public required bool Pagado { get; set; }
    public string? MedioPago { get; set; }
    public string? Referencia { get; set; }
}

/// <summary>
/// Resultado del comando de creación de venta
/// </summary>
public class CreateSaleCommandResult : CommandResult<Guid>
{
}

/// <summary>
/// Comando para marcar venta como pagada
/// </summary>
public class MarkSaleAsPaidCommand : ICommand<MarkSaleAsPaidCommandResult>
{
    public required Guid SaleId { get; set; }
    public string? MedioPago { get; set; }
    public string? Referencia { get; set; }
}

/// <summary>
/// Resultado del comando de marcar venta como pagada
/// </summary>
public class MarkSaleAsPaidCommandResult : CommandResult<Guid>
{
}

