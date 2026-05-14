namespace SystemGym.Application.Validators;

using FluentValidation;
using SystemGym.Application.DTOs.Sales;

/// <summary>
/// Validador para CreateSaleDto
/// </summary>
public class CreateSaleValidator : AbstractValidator<CreateSaleDto>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El cliente es requerido");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("El producto es requerido");

        RuleFor(x => x.FechaVenta)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("La fecha de venta no puede ser futura");

        RuleFor(x => x.MedioPago)
            .Must(medio => medio is null || new[] { "Efectivo", "Tarjeta", "Transferencia", "Cheque" }.Contains(medio))
            .When(x => !string.IsNullOrWhiteSpace(x.MedioPago))
            .WithMessage("El medio de pago debe ser 'Efectivo', 'Tarjeta', 'Transferencia' o 'Cheque'");
    }
}

/// <summary>
/// Validador para MarkSaleAsPaidDto
/// </summary>
public class MarkSaleAsPaidValidator : AbstractValidator<MarkSaleAsPaidDto>
{
    public MarkSaleAsPaidValidator()
    {
        RuleFor(x => x.MedioPago)
            .Must(medio => medio is null || new[] { "Efectivo", "Tarjeta", "Transferencia", "Cheque" }.Contains(medio))
            .When(x => !string.IsNullOrWhiteSpace(x.MedioPago))
            .WithMessage("El medio de pago debe ser 'Efectivo', 'Tarjeta', 'Transferencia' o 'Cheque'");
    }
}
