namespace SystemGym.Application.Features.Products.Commands;

using SystemGym.Application.Abstractions;

/// <summary>
/// Comando para crear un producto
/// </summary>
public class CreateProductCommand : ICommand<CreateProductCommandResult>
{
    public required string Descripcion { get; set; }
    public required decimal Valor { get; set; }
}

/// <summary>
/// Resultado del comando de creación de producto
/// </summary>
public class CreateProductCommandResult : CommandResult<Guid>
{
}
