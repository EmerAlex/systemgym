namespace SystemGym.Application.Features.Inventory.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para ajustar inventario de un producto.
/// </summary>
public class AdjustInventoryCommand : ICommand<AdjustInventoryCommandResult>
{
    public required Guid ProductId { get; set; }
    public required Guid UserId { get; set; }
    public int Cambio { get; set; }
    public required string MotivoAuditoria { get; set; }
}

public class AdjustInventoryCommandResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? LogId { get; set; }
    public int CantidadAnterior { get; set; }
    public int CantidadNueva { get; set; }
    public int Diferencia { get; set; }
    public string? Operacion { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}
