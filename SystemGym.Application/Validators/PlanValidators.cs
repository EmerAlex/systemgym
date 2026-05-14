namespace SystemGym.Application.Validators;

using FluentValidation;
using SystemGym.Application.DTOs.Plans;

/// <summary>
/// Validador para CreatePlanDto
/// </summary>
public class CreatePlanValidator : AbstractValidator<CreatePlanDto>
{
    public CreatePlanValidator()
    {
        RuleFor(x => x.Descripcion).DescripcionRules();
        RuleFor(x => x.TipoPeriodo).TipoPeriodoRules();
        RuleFor(x => x.CantidadIntervalosPeriodo).CantidadIntervalosPeriodoRules();
        RuleFor(x => x.Valor).ValorPositivoRules();
    }
}

/// <summary>
/// Validador para UpdatePlanDto
/// </summary>
public class UpdatePlanValidator : AbstractValidator<UpdatePlanDto>
{
    public UpdatePlanValidator()
    {
        RuleFor(x => x.Descripcion).DescripcionRules();
        RuleFor(x => x.TipoPeriodo).TipoPeriodoRules();
        RuleFor(x => x.CantidadIntervalosPeriodo).CantidadIntervalosPeriodoRules();
        RuleFor(x => x.Valor).ValorPositivoRules();
    }
}
