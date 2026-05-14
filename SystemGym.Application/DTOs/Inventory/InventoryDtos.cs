namespace SystemGym.Application.DTOs.Inventory;

/// <summary>
/// DTO para ajuste de inventario
/// </summary>
public class AdjustInventoryDto
{
    public required Guid ProductId { get; set; }
    public int Cambio { get; set; }
    public required string MotivoAuditoria { get; set; }
}
