namespace SystemGym.Application.Validators;

using FluentValidation;

/// <summary>
/// Extension methods con reglas de validación reutilizables.
/// Evita duplicación entre validators Create/Update de la misma entidad.
/// </summary>
internal static class CommonValidationRules
{
    // ── Descripción genérica ──────────────────────────────────────────────────
    internal static IRuleBuilderOptions<T, string> DescripcionRules<T>(
        this IRuleBuilder<T, string> rule,
        int maxLength = 200)
        => rule
            .NotEmpty().WithMessage("La descripción es requerida")
            .MinimumLength(3).WithMessage("La descripción debe tener al menos 3 caracteres")
            .MaximumLength(maxLength).WithMessage($"La descripción no puede exceder {maxLength} caracteres");

    // ── Valor monetario / numérico positivo ──────────────────────────────────
    internal static IRuleBuilderOptions<T, decimal> ValorPositivoRules<T>(
        this IRuleBuilder<T, decimal> rule)
        => rule
            .GreaterThan(0).WithMessage("El valor debe ser mayor a 0");

    // ── Tipo de período de plan ───────────────────────────────────────────────
    internal static IRuleBuilderOptions<T, string> TipoPeriodoRules<T>(
        this IRuleBuilder<T, string> rule)
        => rule
            .NotEmpty().WithMessage("El tipo de período es requerido")
            .Must(type => type == "Día" || type == "Mes")
            .WithMessage("El tipo de período debe ser 'Día' o 'Mes'");

    // ── Cantidad de intervalos de plan ────────────────────────────────────────
    internal static IRuleBuilderOptions<T, int> CantidadIntervalosPeriodoRules<T>(
        this IRuleBuilder<T, int> rule)
        => rule
            .GreaterThan(0).WithMessage("La cantidad de intervalos debe ser mayor a 0")
            .LessThanOrEqualTo(36).WithMessage("La cantidad de intervalos no puede exceder 36 (3 años)");

    // ── Nombre completo ───────────────────────────────────────────────────────
    internal static IRuleBuilderOptions<T, string> NombreCompletoRules<T>(
        this IRuleBuilder<T, string> rule)
        => rule
            .NotEmpty().WithMessage("El nombre completo es requerido")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

    // ── Celular (opcional) ────────────────────────────────────────────────────
    internal static IRuleBuilderOptions<T, string?> CelularOpcionalRules<T>(
        this IRuleBuilder<T, string?> rule)
        => rule
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("El celular debe ser un número de teléfono válido");

    // ── Tipo de documento de cliente ──────────────────────────────────────────
    internal static IRuleBuilderOptions<T, string> TipoDocumentoRules<T>(
        this IRuleBuilder<T, string> rule)
        => rule
            .NotEmpty().WithMessage("El tipo de documento es requerido")
            .Must(type => type == "CC" || type == "TI" || type == "Pasaporte")
            .WithMessage("El tipo de documento debe ser 'CC', 'TI' o 'Pasaporte'");

    // ── Número de documento ───────────────────────────────────────────────────
    internal static IRuleBuilderOptions<T, string> NumeroDocumentoRules<T>(
        this IRuleBuilder<T, string> rule)
        => rule
            .NotEmpty().WithMessage("El número de documento es requerido")
            .MinimumLength(5).WithMessage("El número de documento debe tener al menos 5 caracteres")
            .MaximumLength(20).WithMessage("El número de documento no puede exceder 20 caracteres");
}
